using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class GradeDirectory(GradesDbContext dbContext) : IGradeDirectory
{
    public async Task<Grade?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.Grades.FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<Grade?> FindStudentAssessmentGradeAsync(int studentId, int assessmentId, CancellationToken ct) =>
        await dbContext.Grades.FirstOrDefaultAsync(g => g.StudentId == studentId && g.AssessmentId == assessmentId, ct);

    public void Add(Grade grade) => dbContext.Grades.Add(grade);
}