using MediatR;
using Microsoft.OpenApi;
using SchoolPortal.Attendances.Infrastructure;
using SchoolPortal.Grades.Infrastructure;
using SchoolPortal.SharedKernel.Behaviors;
using SchoolPortal.Students.Infrastructure;
using SchoolPortal.Web.Endpoints.Attendances;
using SchoolPortal.Web.Endpoints.Students;

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
        
        app.MapControllers(); // needed for Students/Grades/Attendances MVC controllers to route at all
        app.MapDefaultControllerRoute(); // or whatever your existing MVC route mapping is

        app.Run();
    }
}