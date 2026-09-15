using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.AcademicYears;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class AcademicYearDirectory(GradesDbContext dbContext) : IAcademicYearDirectory
{
    public async Task<AcademicYear?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.AcademicYears.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<AcademicYear?> FindCurrentAsync(CancellationToken ct) =>
        await dbContext.AcademicYears.FirstOrDefaultAsync(a => a.IsCurrent, ct);

    public async Task<bool> NameExistsAsync(string name, CancellationToken ct) =>
        await dbContext.AcademicYears.AnyAsync(a => a.Name == name, ct);

    public void Add(AcademicYear academicYear) => dbContext.AcademicYears.Add(academicYear);
    
    // Kept, unlike Grade's — this one takes the entity as a parameter
    // rather than assuming the caller already holds a tracked reference
    // from FindAsync, so an explicit Update() is the safer default here.
    // Still no SaveChangesAsync: staging only.
    public void Update(AcademicYear academicYear) => dbContext.AcademicYears.Update(academicYear);
}