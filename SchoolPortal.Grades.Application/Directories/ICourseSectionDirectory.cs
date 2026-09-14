using SchoolPortal.Grades.Domain.CourseSections;

namespace SchoolPortal.Grades.Application.Directories;

public interface ICourseSectionDirectory
{
    Task<CourseSection?> FindAsync(int id, CancellationToken ct);
    Task<bool> ExistsAsync(int courseId, int termId, CancellationToken ct);
    Task AddAsync(CourseSection courseSection, CancellationToken ct);
}