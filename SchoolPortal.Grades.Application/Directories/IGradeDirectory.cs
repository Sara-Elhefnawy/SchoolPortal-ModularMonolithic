using SchoolPortal.Grades.Domain.Grades;

namespace SchoolPortal.Grades.Application.Directories;

public interface IGradeDirectory
{
    Task<Grade?> FindAsync(int id, CancellationToken ct);
    Task<Grade?> FindAsync(int studentId, int assessmentId, CancellationToken ct);
    Task AddAsync(Grade grade, CancellationToken ct);
    Task UpdateAsync(Grade grade, CancellationToken ct);
}