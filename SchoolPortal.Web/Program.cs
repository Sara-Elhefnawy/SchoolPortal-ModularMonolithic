using Microsoft.OpenApi;
using SchoolPortal.Attendances.Infrastructure;
using SchoolPortal.Grades.Infrastructure;
using SchoolPortal.Students.Infrastructure;
using SchoolPortal.Identities.Infrastructure;
using SchoolPortal.Web.Endpoints.Attendances;
using SchoolPortal.Web.Endpoints.Grades;
using SchoolPortal.Web.Endpoints.Students;
using System.Text.Json.Serialization;
using SchoolPortal.Email.Infrastructure;
using SchoolPortal.Identities.Infrastructure.Persistence.Seeders;
using SchoolPortal.Notifications.Infrastructure;
using SchoolPortal.SharedKernel.Emails.Contracts;
using SchoolPortal.SharedKernel.Emails.Options;
using SchoolPortal.SharedKernel.Identity.Contracts;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Persistence.Seeders;
using SchoolPortal.Web.EmailTemplates;
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
            .AddNotificationsModule(builder.Configuration).AddEmailModule(builder.Configuration);
        
        builder.Services.AddMediatorPattern(typeof(Mediator).Assembly);
        
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
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUser, CurrentUser>();
        
        // It is needed to render each template name ex: "/Views/Shared/EmailTemplates/ResetPassword.cshtml"
        // with its model ex: new ResetPasswordEmailModel { ResetLink = resetLink }
        builder.Services.AddRazorTemplating();
        builder.Services.AddScoped<IEmailTemplateRenderer, RazorEmailTemplateRenderer>();

        builder.Services.AddOptions<AppOptions>().Bind(builder.Configuration.GetSection(AppOptions.SectionName));
        
        var app = builder.Build();

        // Error handling / HSTS
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        // Swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        // Identity database seed
        using (var scope = app.Services.CreateScope())
        {
            foreach (var seeder in scope.ServiceProvider.GetServices<IDataSeeder>().OrderBy(s => s.Order))
                await seeder.SeedAsync(app.Lifetime.ApplicationStopping);
        }

        // HTTP pipeline
        app.UseHttpsRedirection();
        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // Static files
        app.MapStaticAssets();
        
        // Controllers are only responsible for serving the View shell.
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}")
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