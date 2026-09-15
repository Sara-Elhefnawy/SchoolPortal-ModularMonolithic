using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.Courses;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class CourseDirectory(GradesDbContext dbContext) : ICourseDirectory
{
    public async Task<Course?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<bool> ExistsWithCodeAsync(string code, CancellationToken ct) =>
        await dbContext.Courses.AnyAsync(c => c.Code == code, ct);

    public void Add(Course course) => dbContext.Courses.Add(course);
}