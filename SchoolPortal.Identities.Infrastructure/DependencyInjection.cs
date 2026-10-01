using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Application.Feature.Auth.Command.Signup;
using SchoolPortal.Identities.Application.Services;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.Identities.Infrastructure.Authorization;
using SchoolPortal.Identities.Infrastructure.Directories;
using SchoolPortal.Identities.Infrastructure.Persistence;
using SchoolPortal.Identities.Infrastructure.Persistence.Seeders;
using SchoolPortal.Identities.Infrastructure.Services;
using SchoolPortal.SharedKernel.Identity;
using SchoolPortal.SharedKernel.Identity.Contracts;
using SchoolPortal.SharedKernel.Interceptors;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Persistence.Seeders;

namespace SchoolPortal.Identities.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SchoolPortalDb") 
                               ?? throw new InvalidOperationException( "Connection string 'SchoolPortalDb' was not found.");

        services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", IdentityDbContext.Schema));
            options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
        });
        
        // Register ASP.NET Core Identity.
        //
        // This gives us:
        // - UserManager<ApplicationUser>
        // - RoleManager<ApplicationRole>
        // - SignInManager<ApplicationUser>
        // - password hashing
        // - user validation
        // - role management
        // - cookie authentication
        // - token providers
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;

            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

            options.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<IdentityDbContext>().AddDefaultTokenProviders();

        // Replace Identity's default claims-principal factory with our custom implementation.
        // The custom factory adds permission claims based on the user's roles.
        services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

        // Configure the Identity authentication cookie.
        //
        // API requests should NOT receive an HTML redirect, however.
        // For /api requests we return HTTP 401/403 instead.
        services.ConfigureApplicationCookie(options =>
        {
            // If an unauthenticated browser request accesses something requiring authentication,
            // the cookie middleware can redirect the browser to /Account/Login.
            options.LoginPath = "/Account/Login";
            
            // The user is authenticated, but doesn't have permission, so redirect the
            // browser to the AccessDenied page.
            options.AccessDeniedPath = "/Account/AccessDenied";

            // Prevent API clients from receiving a 302 redirect to HTML.
            options.Events.OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    return Task.CompletedTask;
                }

                // Normal MVC/browser behavior.
                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            };

            // Prevent API clients from receiving a 302 redirect to HTML.
            options.Events.OnRedirectToAccessDenied = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                    return Task.CompletedTask;
                }

                // Normal MVC/browser behavior.
                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            };
        });

        // Register an authorization policy for every known permission.
        //      .RequireAuthorization(Permissions.Grades.Create)
        //              => does not mean:
        //      "The user must be a Teacher."
        //              => It means:
        //      "The user's ClaimsPrincipal must contain permission=Grades.Create."
        services.AddAuthorization(options =>
        {
            foreach (var permission in Permissions.All)
            {
                options.AddPolicy(permission, policy => policy.RequireClaim("permission", permission));
            }
        });
        
        services.AddScoped<IIdentityDirectory, IdentityDirectory>();
        
        services.AddScoped<IUserContactDirectory, UserContactDirectory>();
        
        services.AddMediatorPattern(typeof(SignupCommandHandler).Assembly);
        
        services.AddScoped<IDataSeeder, IdentitySeeder>();
        
        services.AddScoped<IUserLinkService, UserLinkService>();
        
        services.AddMediatorPattern(typeof(SignupCommandHandler).Assembly);
        
        return services;
    }
}