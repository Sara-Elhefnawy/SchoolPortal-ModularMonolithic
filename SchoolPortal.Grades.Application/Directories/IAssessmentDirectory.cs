using SchoolPortal.Grades.Application.Features.Assessments.Query.GetByCourseSection;
using SchoolPortal.Grades.Domain.Assessments;

namespace SchoolPortal.Grades.Application.Directories;

public interface IAssessmentDirectory
{
    Task<Assessment?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<GetAssessmentsByCourseSectionQueryDto>> GetBySectionAsync(int courseSectionId, CancellationToken ct);
    void Add(Assessment assessment);
}