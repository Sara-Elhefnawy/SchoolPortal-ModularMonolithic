using SchoolPortal.Grades.Domain.Terms;

namespace SchoolPortal.Grades.Application.Directories;

public interface ITermDirectory
{
    Task<Term?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> AcademicYearAndNameExistsAsync(int academicYearId, string name, CancellationToken ct);
    void Add(Term term);
}