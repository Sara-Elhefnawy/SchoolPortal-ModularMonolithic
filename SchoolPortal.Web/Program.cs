using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi;
using SchoolPortal.Attendances.Infrastructure;
using SchoolPortal.Email.Infrastructure;
using SchoolPortal.Grades.Infrastructure;
using SchoolPortal.Identities.Infrastructure;
using SchoolPortal.Notifications.Infrastructure;
using SchoolPortal.SharedKernel.Emails.Contracts;
using SchoolPortal.SharedKernel.Emails.Options;
using SchoolPortal.SharedKernel.Identity.Contracts;
using SchoolPortal.SharedKernel.Interceptors;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Students.Infrastructure;
using SchoolPortal.Web.EmailTemplates;
using SchoolPortal.Web.Endpoints.Attendances;
using SchoolPortal.Web.Endpoints.Grades;
using SchoolPortal.Web.Endpoints.Identity.Account;
using SchoolPortal.Web.Endpoints.Identity.Permission;
using SchoolPortal.Web.Endpoints.Identity.Roles;
using SchoolPortal.Web.Endpoints.Identity.Users;
using SchoolPortal.Web.Endpoints.Students;
using SchoolPortal.Web.Extensions;
using SchoolPortal.Web.Identity.Users;

namespace SchoolPortal.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.ConfigureApplicationCookie(options =>
        {
            // WHY HttpOnly: JavaScript can't read the auth cookie.
            options.Cookie.HttpOnly = true;

            // WHY Always: the cookie is only ever sent over HTTPS. This works behind Nginx only because
            // ForwardedHeaders makes the app see the request as https.
            // SameAsRequest in Development keeps http://localhost working.
            options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;

            // WHY Lax and not Strict: your confirm-email and reset-password links are cross-site top-level
            // navigations from the user's mail client. Strict cookies aren't sent on those, so users would
            // look logged out on arrival. Lax blocks cross-site POSTs (the CSRF vector) and still allows link clicks.
            options.Cookie.SameSite = SameSiteMode.Lax;
        });

        builder.Services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.HttpOnly = true;
            // WHY same Always rule: the antiforgery cookie must obey the same HTTPS-only policy.
            options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;
        });
        
        // WHY a mounted volume: keys must outlive the container. The non-root "app" user needs write
        // access to /keys, so create it in the Dockerfile and chown it to app.
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/keys"))
            .SetApplicationName("SchoolPortal");
        
        // Configure Data Protection to persist keys to a specific folder
        builder.Services.AddDataProtection()
            // Why SetApplicationName: keys are isolated per app name. Pinning it means a
            // changed path or assembly name can't silently orphan your existing keys.
            .SetApplicationName("SchoolPortal")
            // Why /keys: it is the volume mount, and it is owned by the non-root app user.
            .PersistKeysToFileSystem(new DirectoryInfo("/keys"));

        builder.Services.AddControllersWithViews();

        builder.Services.AddStudentsModule(builder.Configuration).AddAttendanceModule(builder.Configuration)
            .AddGradesModule(builder.Configuration).AddIdentityModule(builder.Configuration)
            .AddNotificationsModule(builder.Configuration).AddEmailModule(builder.Configuration);

        builder.Services.AddMediatorPattern(typeof(Mediator).Assembly);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "SchoolPortal API", Version = "v1" });
        });

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUser, CurrentUser>();

        // Needed to render each template (e.g. ResetPassword.cshtml) with its model.
        builder.Services.AddRazorTemplating();
        builder.Services.AddScoped<IEmailTemplateRenderer, RazorEmailTemplateRenderer>();

        builder.Services.AddOptions<AppOptions>().Bind(builder.Configuration.GetSection(AppOptions.SectionName));
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddScoped<AuditInterceptor>();

        builder.Services.AddAppAntiforgery();
        builder.Services.AddAppHealthChecks();

        var app = builder.Build();

        // Why first: everything after this (HSTS, auth, antiforgery) reads the real scheme and client IP.
        var forwardedOptions = new ForwardedHeadersOptions
        {
            // XForwardedProto = "the visitor used https" (needed for Secure cookies).
            // XForwardedFor   = the visitor's real IP (needed for login rate limiting later).
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        };

        // Why Clear: by default only localhost is trusted, so headers from the Nginx container
        // would be ignored. Clearing the lists makes the app trust the proxy in front of it.
        // This is safe ONLY because port 8080 is not published to the internet in production,
        // so only Nginx can reach the app.
        forwardedOptions.KnownIPNetworks.Clear();   // use KnownNetworks if this name doesn't compile
        forwardedOptions.KnownProxies.Clear();

        app.UseForwardedHeaders(forwardedOptions);

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        await app.SeedDatabaseAsync();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();
        app.UseCsrfValidation();

        app.MapControllers();
        app.MapStaticAssets();

        // Controllers only serve the View shell.
        app.MapControllerRoute(name: "default", pattern: "{controller=Account}/{action=Login}/{id?}")
            .WithStaticAssets();

        app.MapAppHealthChecks();
        app.MapAntiforgeryTokenEndpoint();

        app.MapStudentEndpoints();
        app.MapAttendanceEndpoints();
        app.MapGradesEndpoints();
        app.MapIdentityEndpoints();
        app.MapRoleEndpoints();
        app.MapAccountEndpoints();
        app.MapPermissionEndpoints();

        app.Run();
    }
}