using SchoolPortal.Grades.Application.Features.CourseSections.Query.GetByTerm;
using SchoolPortal.Grades.Application.Features.CourseSections.Query.GetRankings;
using SchoolPortal.Grades.Domain.CourseSections;

namespace SchoolPortal.Grades.Application.Directories;

public interface ICourseSectionDirectory
{
    Task<IReadOnlyList<GetCourseSectionRankingsQueryDto>> GetRankingsAsync(int courseSectionId, CancellationToken ct);
    Task<CourseSection?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<GetCourseSectionsByTermQueryDto>> GetByTermIdAsync(int termId, CancellationToken ct);
    Task<bool> CourseAndTermExistsAsync(int courseId, int termId, CancellationToken ct);
    void Add(CourseSection courseSection);
}