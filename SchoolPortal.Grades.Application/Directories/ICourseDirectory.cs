using SchoolPortal.Grades.Domain.Courses;

namespace SchoolPortal.Grades.Application.Directories;

public interface ICourseDirectory
{
    Task<Course?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> ExistsWithCodeAsync(string code, CancellationToken ct);
    void Add(Course course);
}