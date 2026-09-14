using SchoolPortal.Grades.Domain.Terms;

namespace SchoolPortal.Grades.Application.Directories;

public interface ITermDirectory
{
    Task<Term?> FindAsync(int id, CancellationToken ct);
    Task<bool> ExistsAsync(int academicYearId, string name, CancellationToken ct);
    Task AddAsync(Term term, CancellationToken ct);
}