using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.AcademicYears;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class AcademicYearDirectory(GradesDbContext dbContext) : IAcademicYearDirectory
{
    public Task<AcademicYear?> FindAsync(int id, CancellationToken ct)
        => dbContext.AcademicYears.FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<AcademicYear?> FindCurrentAsync(CancellationToken ct)
        => dbContext.AcademicYears.FirstOrDefaultAsync(a => a.IsCurrent, ct);

    public Task<bool> ExistsWithNameAsync(string name, CancellationToken ct)
        => dbContext.AcademicYears.AnyAsync(a => a.Name == name, ct);

    public Task AddAsync(AcademicYear academicYear, CancellationToken ct)
    {
        dbContext.AcademicYears.Add(academicYear);
        return dbContext.SaveChangesAsync(ct);
    }

    public Task UpdateAsync(AcademicYear academicYear, CancellationToken ct)
    {
        dbContext.AcademicYears.Update(academicYear);
        return dbContext.SaveChangesAsync(ct);
    }
}