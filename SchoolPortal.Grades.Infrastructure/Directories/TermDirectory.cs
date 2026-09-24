using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Features.Terms.Query.GetByAcademicYear;
using SchoolPortal.Grades.Domain.Terms;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class TermDirectory(GradesDbContext dbContext) : ITermDirectory
{
    public async Task<Term?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.Terms.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<GetTermsByAcademicYearQueryDto>>
        GetByAcademicYearAsync(int academicYearId, CancellationToken ct) =>
        await dbContext.Terms.AsNoTracking().Where(x => x.AcademicYearId == academicYearId).OrderBy(x => x.StartDate)
            .Select(x => new GetTermsByAcademicYearQueryDto(x.Id, x.Name, x.Type.ToString(), x.StartDate, x.EndDate,
                x.AcademicYearId, x.HasEnded)).ToListAsync(ct);

    public async Task<bool> AcademicYearAndNameExistsAsync(int academicYearId, string name, CancellationToken ct) =>
        await dbContext.Terms.AnyAsync(t => t.AcademicYearId == academicYearId && t.Name == name, ct);

    public void Add(Term term) => dbContext.Terms.Add(term);
}