using MediatR;
using Microsoft.OpenApi;
using SchoolPortal.Attendances.Infrastructure;
using SchoolPortal.Grades.Infrastructure;
using SchoolPortal.SharedKernel.Behaviors;
using SchoolPortal.Students.Infrastructure;
using SchoolPortal.Identities.Infrastructure;
using SchoolPortal.Web.Endpoints.Attendances;
using SchoolPortal.Web.Endpoints.Grades;
using SchoolPortal.Web.Endpoints.Students;
using System.Text.Json.Serialization;
using SchoolPortal.Attendances.Application.Features.Attendances.Command.Cancel;
using SchoolPortal.Grades.Application.Features.Grades.Command.Create;
using SchoolPortal.Identities.Application.Feature.Auth.Command.Signup;
using SchoolPortal.Identities.Infrastructure.Persistence.Seeders;
using SchoolPortal.Notifications.Application.EventHandlers.Attendances.SessionOccurrences;
using SchoolPortal.Notifications.Application.EventHandlers.Grades;
using SchoolPortal.Notifications.Application.EventHandlers.Students;
using SchoolPortal.Notifications.Infrastructure;
using SchoolPortal.SharedKernel.Identity.Users;
using SchoolPortal.Students.Application.Features.Students.Commands.UpdatePhone;
using SchoolPortal.Web.Endpoints.Identity.Account;
using SchoolPortal.Web.Endpoints.Identity.Permission;
using SchoolPortal.Web.Endpoints.Identity.Roles;
using SchoolPortal.Web.Endpoints.Identity.Users;
using SchoolPortal.Web.Identity.Users;

namespace SchoolPortal.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Controllers are used for serving the HTML/View shell.
        builder.Services.AddControllersWithViews();

        builder.Services.AddStudentsModule(builder.Configuration).AddAttendanceModule(builder.Configuration)
            .AddGradesModule(builder.Configuration).AddIdentityModule(builder.Configuration)
            .AddNotificationsModule(builder.Configuration);
        
        // MediatR pipeline
        builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        // API / Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SchoolPortal API",
                Version = "v1"
            });
        });
        
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(
                new JsonStringEnumConverter());
        });
        
        // This is the ONE place in the whole solution allowed to reference every module's
        // Application assembly at once — because it's the composition root, not a module.
        // Each module's own DI setup only ever registers ITS OWN services (directories,
        // unit of work) — never MediatR's cross-module scanning.
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(UpdateStudentPhoneCommandHandler).Assembly, typeof(StudentPhoneNumberUpdatedHandler).Assembly,
            typeof(CreateGradeCommandHandler).Assembly, typeof(GradeCreatedHandler).Assembly,
            typeof(CancelSessionOccurrenceCommandHandler).Assembly,
            typeof(SessionOccurrenceCancelledHandler).Assembly,
            typeof(SignupCommandHandler).Assembly));
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUser, CurrentUser>();
        
        var app = builder.Build();

        // Error handling / HSTS
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        // Swagger
        if (!app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        // Identity database seed
        using (var scope = app.Services.CreateScope())
        {
            await IdentitySeeder.SeedAsync(scope.ServiceProvider);
        }

        // HTTP pipeline
        app.UseHttpsRedirection();
        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();

        // Static files
        app.MapStaticAssets();
        
        // Controllers are only responsible for serving the View shell.
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        
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