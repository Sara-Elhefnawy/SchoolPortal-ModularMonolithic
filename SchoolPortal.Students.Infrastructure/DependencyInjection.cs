using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Infrastructure.Directories;

namespace SchoolPortal.Students.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStudentsModule(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SchoolPortalDb") 
                               ?? throw new InvalidOperationException( "Connection string 'SchoolPortalDb' was not found.");

        services.AddDbContext<StudentsDbContext>(opt =>
            opt.UseNpgsql(connectionString,
                // tells EF to only ever touch the "students" schema 
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "students")));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(DependencyInjection).Assembly,
            typeof(IStudentDirectory).Assembly));

        // This is the ONE thing this module exposes to other modules.
        // Grades will depend on this interface — never on StudentsDbContext directly.
        services.AddScoped<IStudentDirectory, StudentDirectory>();

        return services;
    }
}