using SchoolPortal.Grades.Domain.AcademicYears;

namespace SchoolPortal.Grades.Application.Directories;

public interface IAcademicYearDirectory
{
    Task<AcademicYear?> FindAsync(int id, CancellationToken ct);
    Task<AcademicYear?> FindCurrentAsync(CancellationToken ct);
    Task<bool> ExistsWithNameAsync(string name, CancellationToken ct);
    Task AddAsync(AcademicYear academicYear, CancellationToken ct);
    Task UpdateAsync(AcademicYear academicYear, CancellationToken ct);
}