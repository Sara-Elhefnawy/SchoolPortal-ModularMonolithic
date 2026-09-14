using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Domain.AcademicYears;
using SchoolPortal.Grades.Domain.Assessments;
using SchoolPortal.Grades.Domain.Courses;
using SchoolPortal.Grades.Domain.CourseSections;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.Grades.Domain.Terms;

namespace SchoolPortal.Grades.Infrastructure.Persistence;

public sealed class GradesDbContext(DbContextOptions<GradesDbContext> options) : DbContext(options)
{
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Term> Terms => Set<Term>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseSection> CourseSections => Set<CourseSection>();
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("grades");
        
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GradesDbContext).Assembly);
    }
}