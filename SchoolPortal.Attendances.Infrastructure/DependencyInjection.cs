using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Application.Features.Sessions.Command.Create;
using SchoolPortal.Attendances.Application.Persistence;
using SchoolPortal.Attendances.Infrastructure.Directories;
using SchoolPortal.Attendances.Infrastructure.Persistence;
using SchoolPortal.Attendances.Infrastructure.Persistence.Seeders;
using SchoolPortal.SharedKernel.Interceptors;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Persistence.Seeders;

namespace SchoolPortal.Attendances.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAttendanceModule(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SchoolPortalDb") ??
                               throw new InvalidOperationException("Connection string 'SchoolPortalDb' was not found.");

        services.AddDbContext<AttendancesDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", AttendancesDbContext.Schema));
            options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
        });
        
        services.AddScoped<IAttendanceDirectory, AttendanceDirectory>();
        services.AddScoped<ISessionDirectory, SessionDirectory>();
        services.AddScoped<ISessionOccurrenceDirectory, SessionOccurrenceDirectory>();

        // Registers every AbstractValidator<T> found in this assembly
        services.AddValidatorsFromAssembly(typeof(CreateSessionCommandValidator).Assembly);
        
        services.AddMediatorPattern(typeof(CreateSessionCommandHandler).Assembly);

        services.AddScoped<IAttendancesUnitOfWork, AttendancesUnitOfWork>();
        
        services.AddScoped<IDataSeeder, AttendancesSeeder>();

        services.AddHealthChecks().AddDbContextCheck<AttendancesDbContext>("attendance-db", tags: ["ready"]);
        
        return services;
    }
}