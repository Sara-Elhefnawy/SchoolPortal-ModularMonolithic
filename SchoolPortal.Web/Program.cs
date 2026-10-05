using System.Text.Json.Serialization;
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

        // Why: behind a reverse proxy the app sees HTTP even though the user used HTTPS.
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseHttpsRedirection();
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