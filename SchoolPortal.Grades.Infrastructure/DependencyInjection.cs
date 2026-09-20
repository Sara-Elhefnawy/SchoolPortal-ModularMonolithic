using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Features.AcademicYears.Command.Create;
using SchoolPortal.Grades.Application.Persistence;
using SchoolPortal.Grades.Application.Reporting.Queries;
using SchoolPortal.Grades.Infrastructure.Directories;
using SchoolPortal.Grades.Infrastructure.Persistence;
using SchoolPortal.Grades.Infrastructure.Reporting;

namespace SchoolPortal.Grades.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGradesModule(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SchoolPortalDb") ??
                               throw new InvalidOperationException("Connection string 'SchoolPortalDb' was not found.");

        services.AddDbContext<GradesDbContext>(opt => opt.UseNpgsql(connectionString,
            // tells EF to only ever touch the "grades" schema 
            npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", GradesDbContext.Schema)));
        
        services.AddScoped<IAcademicYearDirectory, AcademicYearDirectory>();
        services.AddScoped<ITermDirectory, TermDirectory>();
        services.AddScoped<ICourseDirectory, CourseDirectory>();
        services.AddScoped<IGradeDirectory, GradeDirectory>();
        services.AddScoped<ICourseSectionDirectory, CourseSectionDirectory>();
        services.AddScoped<IAssessmentDirectory, AssessmentDirectory>();

        // Registers every AbstractValidator<T> found in this assembly
        services.AddValidatorsFromAssembly(typeof(CreateAcademicYearCommandValidator).Assembly);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(DependencyInjection).Assembly,
            typeof(IAcademicYearDirectory).Assembly));

        services.AddScoped<IGradeReportingQueries, GradeReportingQueries>();

        services.AddNpgsqlDataSource(connectionString);
        
        // Same instance, two doors in: handlers that need Directories get GradesDbContext-backed interfaces as before
        //      handlers that only need to commit get IGradesUnitOfWork.
        // Both resolve to the SAME DbContext instance within a request (AddDbContext is scoped by default), so
        // there's exactly one ChangeTracker and one transaction either way.
        services.AddScoped<IGradesUnitOfWork, GradesUnitOfWork>();
        
        return services;
    }
}