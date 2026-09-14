using SchoolPortal.Grades.Domain.Courses;

namespace SchoolPortal.Grades.Application.Directories;

public interface ICourseDirectory
{
    Task<Course?> FindAsync(int id, CancellationToken ct);
    Task<bool> ExistsWithCodeAsync(string code, CancellationToken ct);
    Task AddAsync(Course course, CancellationToken ct);
}