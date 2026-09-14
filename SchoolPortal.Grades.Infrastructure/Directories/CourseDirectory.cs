using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.Courses;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class CourseDirectory(GradesDbContext db) : ICourseDirectory
{
    public Task<Course?> FindAsync(int id, CancellationToken ct)
        => db.Courses.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<bool> ExistsWithCodeAsync(string code, CancellationToken ct)
        => db.Courses.AnyAsync(c => c.Code == code, ct);

    public Task AddAsync(Course course, CancellationToken ct)
    {
        db.Courses.Add(course);
        return db.SaveChangesAsync(ct);
    }
}