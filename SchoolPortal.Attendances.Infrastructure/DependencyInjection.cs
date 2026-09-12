using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Attendances.Application.Abstractions;
using SchoolPortal.Attendances.Infrastructure.Directories;

namespace SchoolPortal.Attendances.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAttendanceModule(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SchoolPortalDb")
                               ?? throw new InvalidOperationException("Connection string 'SchoolPortalDb' was not found.");

        services.AddDbContext<AttendancesDbContext>(opt =>
            opt.UseNpgsql(connectionString,
                b => b.MigrationsHistoryTable("__EFMigrationsHistory", "attendance")));

        services.AddScoped<IAttendanceDirectory, AttendanceDirectory>();
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(DependencyInjection).Assembly,
            typeof(IAttendanceDirectory).Assembly));
        
        return services;
    }
}