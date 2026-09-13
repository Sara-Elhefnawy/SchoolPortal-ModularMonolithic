using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Attendances.Application.DbContexts;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Application.Features.Sessions.Command.Create;
using SchoolPortal.Attendances.Infrastructure.Directories;
using SchoolPortal.Attendances.Infrastructure.Persistence;

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
        
        services.AddScoped<IAttendancesDbContext>(sp => sp.GetRequiredService<AttendancesDbContext>());

        services.AddValidatorsFromAssembly(typeof(CreateSessionCommandValidator).Assembly);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(DependencyInjection).Assembly,
            typeof(IAttendanceDirectory).Assembly));
        
        return services;
    }
}