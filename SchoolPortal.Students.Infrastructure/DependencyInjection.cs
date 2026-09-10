using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SchoolPortal.Students.Infrastructure;

public static class DependencyInjection
{
    // This is the ONLY entry point the host needs to know about for this module.
    // Everything internal to Students stays internal to Students.
    public static IServiceCollection AddStudentsModule(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SchoolPortalDb") 
                               ?? throw new InvalidOperationException( "Connection string 'SchoolPortalDb' was not found.");
        
        // EF Core here specifically because Students is simple CRUD-shaped
        // (add student, enroll, edit profile) — no complex aggregation, so
        // EF's change tracking + migrations save more boilerplate than they cost.
        services.AddDbContext<StudentsDbContext>(opt =>
            opt.UseNpgsql(connectionString,
                // tells EF to only ever touch the "students" schema —
                // this is what makes Rule 3 (own schema) actually enforced,
                // not just a naming convention you have to remember.
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "students")));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // This is the ONE thing this module exposes to other modules.
        // Grades will depend on this interface — never on StudentsDbContext directly.
        // ########################################services.AddScoped<IStudentDirectory, StudentDirectory>();

        return services;
    }
}