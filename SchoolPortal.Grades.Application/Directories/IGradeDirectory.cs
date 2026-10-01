using SchoolPortal.Grades.Application.Features.Grades.Query.GetByAssessment;
using SchoolPortal.Grades.Application.Features.Grades.Query.GetForStudent;
using SchoolPortal.Grades.Domain.Grades;

namespace SchoolPortal.Grades.Application.Directories;

public interface IGradeDirectory
{
    Task<Grade?> GetByIdAsync(int id, CancellationToken ct);
    Task<string?> GetAssessmentNameAsync(int assessmentId, CancellationToken ct);
    Task<IReadOnlyList<GetGradesByAssessmentQueryDto>> GetByAssessmentIdAsync(int assessmentId, CancellationToken ct);
    Task<IReadOnlyList<GetGradesForStudentQueryDto>> GetForStudentAsync(int studentId, CancellationToken ct);
    Task<Grade?> FindStudentAssessmentGradeAsync(int studentId, int assessmentId, CancellationToken ct);
    void Add(Grade grade);
}