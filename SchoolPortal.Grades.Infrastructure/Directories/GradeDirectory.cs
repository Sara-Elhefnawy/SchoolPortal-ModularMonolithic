using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class GradeDirectory(GradesDbContext db) : IGradeDirectory
{
    public Task<Grade?> FindAsync(int id, CancellationToken ct)
        => db.Grades.FirstOrDefaultAsync(g => g.Id == id, ct);

    public Task<Grade?> FindAsync(int studentId, int assessmentId, CancellationToken ct)
        => db.Grades.FirstOrDefaultAsync(g => g.StudentId == studentId && g.AssessmentId == assessmentId, ct);

    public Task AddAsync(Grade grade, CancellationToken ct)
    {
        db.Grades.Add(grade);
        return db.SaveChangesAsync(ct);
    }

    public Task UpdateAsync(Grade grade, CancellationToken ct)
    {
        db.Grades.Update(grade);
        return db.SaveChangesAsync(ct);
    }
}