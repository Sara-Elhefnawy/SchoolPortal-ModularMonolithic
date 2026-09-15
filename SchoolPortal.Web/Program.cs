using MediatR;
using Microsoft.OpenApi;
using SchoolPortal.Attendances.Infrastructure;
using SchoolPortal.Grades.Infrastructure;
using SchoolPortal.SharedKernel.Behaviors;
using SchoolPortal.Students.Infrastructure;
using SchoolPortal.Web.Endpoints.Attendances;
using SchoolPortal.Web.Endpoints.Grades;
using SchoolPortal.Web.Endpoints.Students;
using System.Text.Json.Serialization;
using SchoolPortal.Attendances.Application.Features.Attendances.Command.Cancel;
using SchoolPortal.Grades.Application.Features.Grades.Command.Create;
using SchoolPortal.Notifications.Application.EventHandlers.Attendances.SessionOccurrences;
using SchoolPortal.Notifications.Application.EventHandlers.Grades;
using SchoolPortal.Notifications.Application.EventHandlers.Students;
using SchoolPortal.Students.Application.Features.Students.Commands.UpdatePhone;

namespace SchoolPortal.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddStudentsModule(builder.Configuration).AddAttendanceModule(builder.Configuration).AddGradesModule(builder.Configuration);
        
        builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
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
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(typeof(UpdateStudentPhoneCommandHandler).Assembly,
                typeof(StudentPhoneNumberUpdatedHandler).Assembly,
                typeof(CreateGradeCommandHandler).Assembly,
                typeof(GradeCreatedHandler).Assembly,
                typeof(CancelSessionOccurrenceCommandHandler).Assembly,
                typeof(SessionOccurrenceCancelledHandler).Assembly));
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        if (!app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        
        app.MapStudentEndpoints();
        app.MapAttendanceEndpoints();
        app.MapGradesEndpoints();
        
        app.MapControllers(); // needed for Students/Grades/Attendances MVC controllers to route at all
        app.MapDefaultControllerRoute(); // or whatever your existing MVC route mapping is

        app.Run();
    }
}