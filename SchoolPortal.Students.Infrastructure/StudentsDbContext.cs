using Microsoft.EntityFrameworkCore;
using SchoolPortal.Students.Domain.Enrollments;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Infrastructure;

public sealed class StudentsDbContext(DbContextOptions<StudentsDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("students");
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudentsDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}