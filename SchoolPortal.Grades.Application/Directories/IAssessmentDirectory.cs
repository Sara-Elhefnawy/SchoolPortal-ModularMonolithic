using SchoolPortal.Grades.Domain.Assessments;

namespace SchoolPortal.Grades.Application.Directories;

public interface IAssessmentDirectory
{
    Task<Assessment?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Assessment>> GetBySectionAsync(int courseSectionId, CancellationToken ct);
    void Add(Assessment assessment);
}