using Microsoft.EntityFrameworkCore;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Infrastructure;

public sealed class StudentsDbContext(DbContextOptions<StudentsDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudentsDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}