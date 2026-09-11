using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Attendance.Application.Abstractions;
using SchoolPortal.Attendance.Infrastructure.Directories;

namespace SchoolPortal.Attendance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAttendanceModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AttendanceDbContext>(opt =>
            opt.UseNpgsql(config.GetConnectionString("SchoolPortalDb"),
                b => b.MigrationsHistoryTable("__EFMigrationsHistory", "attendance")));

        services.AddScoped<IAttendanceDirectory, AttendanceDirectory>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}