using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Application.Features.Students.Commands.Create;
using SchoolPortal.Students.Application.Persistence;
using SchoolPortal.Students.Infrastructure.Directories;
using SchoolPortal.Students.Infrastructure.Persistence;

namespace SchoolPortal.Students.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStudentsModule(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SchoolPortalDb") 
                               ?? throw new InvalidOperationException( "Connection string 'SchoolPortalDb' was not found.");

        services.AddDbContext<StudentsDbContext>(opt => opt.UseNpgsql(connectionString,
            // tells EF to only ever touch the "students" schema 
            npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", StudentsDbContext.Schema)));

        services.AddScoped<IStudentDirectory, StudentDirectory>();
        services.AddScoped<IEnrollmentDirectory, EnrollmentDirectory>();
        
        // Registers every AbstractValidator<T> found in this assembly
        services.AddValidatorsFromAssembly(typeof(CreateStudentCommandValidator).Assembly);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(DependencyInjection).Assembly,
            typeof(IStudentDirectory).Assembly));
        
        services.AddScoped<IStudentsUnitOfWork, StudentsUnitOfWork>();
        
        return services;
    }
}