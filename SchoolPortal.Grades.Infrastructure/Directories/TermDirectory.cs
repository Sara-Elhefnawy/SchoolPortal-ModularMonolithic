using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.Terms;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class TermDirectory(GradesDbContext dbContext) : ITermDirectory
{
    public async Task<Term?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.Terms.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<bool> AcademicYearAndNameExistsAsync(int academicYearId, string name, CancellationToken ct) =>
        await dbContext.Terms.AnyAsync(t => t.AcademicYearId == academicYearId && t.Name == name, ct);

    public void Add(Term term) => dbContext.Terms.Add(term);
}