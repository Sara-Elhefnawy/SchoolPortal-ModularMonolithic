using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SchoolPortal.Grades.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGradesModule(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SchoolPortalDb")
                               ?? throw new InvalidOperationException("Connection string 'SchoolPortalDb' was not found.");

        services.AddDbContext<GradesDbContext>(opt =>
            opt.UseNpgsql(connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "grades")));

        return services;
    }
}