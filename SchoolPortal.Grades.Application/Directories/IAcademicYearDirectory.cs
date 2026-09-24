using SchoolPortal.Grades.Application.Features.AcademicYears.Query.GetAll;
using SchoolPortal.Grades.Domain.AcademicYears;

namespace SchoolPortal.Grades.Application.Directories;

public interface IAcademicYearDirectory
{
    Task<IReadOnlyList<GetAllAcademicYearsQueryDto>> GetAllAsync(CancellationToken ct);
    Task<AcademicYear?> GetByIdAsync(int id, CancellationToken ct);
    Task<AcademicYear?> FindCurrentAsync(CancellationToken ct);
    Task<bool> NameExistsAsync(string name, CancellationToken ct);
    void Add(AcademicYear academicYear);
    void Update(AcademicYear academicYear);
}