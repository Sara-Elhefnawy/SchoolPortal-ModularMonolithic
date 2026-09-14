using SchoolPortal.Grades.Domain.Assessments;

namespace SchoolPortal.Grades.Application.Directories;

public interface IAssessmentDirectory
{
    Task<Assessment?> FindAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Assessment>> GetBySectionAsync(int courseSectionId, CancellationToken ct);
    Task AddAsync(Assessment assessment, CancellationToken ct);
}