using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.Assessments;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class AssessmentDirectory(GradesDbContext db) : IAssessmentDirectory
{
    public Task<Assessment?> FindAsync(int id, CancellationToken ct)
        => db.Assessments.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IReadOnlyList<Assessment>> GetBySectionAsync(int courseSectionId, CancellationToken ct)
        => await db.Assessments.Where(a => a.CourseSectionId == courseSectionId).ToListAsync(ct);

    public Task AddAsync(Assessment assessment, CancellationToken ct)
    {
        db.Assessments.Add(assessment);
        return db.SaveChangesAsync(ct);
    }
}