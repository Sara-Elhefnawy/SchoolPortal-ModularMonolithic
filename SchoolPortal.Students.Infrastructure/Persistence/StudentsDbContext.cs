using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.SharedKernel.Persistence;
using SchoolPortal.Students.Domain.Enrollments;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Infrastructure.Persistence;

public sealed class StudentsDbContext(
    DbContextOptions<StudentsDbContext> options, 
    IPublisher publisher)
    : ModuleDbContext(options, publisher)
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