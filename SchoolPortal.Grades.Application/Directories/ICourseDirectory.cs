using SchoolPortal.Grades.Application.Features.Courses.Query.GetAll;
using SchoolPortal.Grades.Domain.Courses;

namespace SchoolPortal.Grades.Application.Directories;

public interface ICourseDirectory
{
    Task<IReadOnlyList<GetAllCoursesQueryDto>> GetAllAsync(CancellationToken ct);
    Task<Course?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> ExistsWithCodeAsync(string code, CancellationToken ct);
    void Add(Course course);
}