using Microsoft.EntityFrameworkCore;
using SchoolPortal.Students.Domain.Enrollments;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.DbContexts;

// Application only knows "something that exposes these DbSets and can save"
// StudentsDbContext in Infrastructure implements this;
//      the handler below depends on the interface.
public interface IStudentsDbContext
{
    DbSet<Student> Students { get; }
    DbSet<Enrollment> Enrollments { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}