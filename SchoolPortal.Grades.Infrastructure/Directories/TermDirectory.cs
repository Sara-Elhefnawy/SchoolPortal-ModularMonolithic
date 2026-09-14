using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.Terms;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class TermDirectory(GradesDbContext dbContext) : ITermDirectory
{
    public Task<Term?> FindAsync(int id, CancellationToken ct)
        => dbContext.Terms.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<bool> ExistsAsync(int academicYearId, string name, CancellationToken ct)
        => dbContext.Terms.AnyAsync(t => t.AcademicYearId == academicYearId && t.Name == name, ct);

    public Task AddAsync(Term term, CancellationToken ct)
    {
        dbContext.Terms.Add(term);
        return dbContext.SaveChangesAsync(ct);
    }
}