using SchoolPortal.Grades.Domain.CourseSections;

namespace SchoolPortal.Grades.Application.Directories;

public interface ICourseSectionDirectory
{
    Task<CourseSection?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> CourseAndTermExistsAsync(int courseId, int termId, CancellationToken ct);
    void Add(CourseSection courseSection);
}