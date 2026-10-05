using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.SharedKernel.Interceptors;
using SchoolPortal.SharedKernel.Lookups;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Persistence.Seeders;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Application.Features.Students.Commands.Create;
using SchoolPortal.Students.Application.Persistence;
using SchoolPortal.Students.Infrastructure.Directories;
using SchoolPortal.Students.Infrastructure.Persistence;
using SchoolPortal.Students.Infrastructure.Persistence.Lookups;
using SchoolPortal.Students.Infrastructure.Persistence.Seeders;

namespace SchoolPortal.Students.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStudentsModule(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SchoolPortalDb") 
                               ?? throw new InvalidOperationException( "Connection string 'SchoolPortalDb' was not found.");

        services.AddDbContext<StudentsDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", StudentsDbContext.Schema));
            options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
        });
        
        services.AddScoped<IStudentDirectory, StudentDirectory>();
        services.AddScoped<IEnrollmentDirectory, EnrollmentDirectory>();
        
        // Registers every AbstractValidator<T> found in this assembly
        services.AddValidatorsFromAssembly(typeof(CreateStudentCommandValidator).Assembly);

        services.AddMediatorPattern(typeof(CreateStudentCommandHandler).Assembly);
        
        services.AddScoped<IStudentsUnitOfWork, StudentsUnitOfWork>();
        
        services.AddScoped<IDataSeeder, StudentsSeeder>();
        
        services.AddScoped<IStudentLookup, StudentLookup>();

        services.AddHealthChecks().AddDbContextCheck<StudentsDbContext>("students-db", tags: ["ready"]);
        
        return services;
    }
}