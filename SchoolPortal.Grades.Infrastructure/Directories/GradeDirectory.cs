using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Features.Grades.Query.GetByAssessment;
using SchoolPortal.Grades.Application.Features.Grades.Query.GetForStudent;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class GradeDirectory(GradesDbContext dbContext) : IGradeDirectory
{
    public async Task<Grade?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.Grades.FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<string?> GetAssessmentNameAsync(int assessmentId, CancellationToken ct) =>
        await dbContext.Assessments.AsNoTracking().Where(a => a.Id == assessmentId).Select(a => a.Name)
            .FirstOrDefaultAsync(ct);
    
    public async Task<IReadOnlyList<GetGradesByAssessmentQueryDto>> GetByAssessmentIdAsync(int assessmentId,
        CancellationToken ct)
    {
        var grades = await dbContext.Grades.AsNoTracking().Where(g => g.AssessmentId == assessmentId).ToListAsync(ct);

        var assessment = await dbContext.Assessments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == assessmentId, ct);

        var dtos = grades.Select(g => new GetGradesByAssessmentQueryDto(g.Id, g.StudentId,
            $"Student #{g.StudentId}", // replace with real name lookup
            g.AssessmentId, assessment?.Name ?? string.Empty, g.Score, g.Status.ToString())).ToList();

        return dtos;
    }

    public async Task<IReadOnlyList<GetGradesForStudentQueryDto>>
        GetForStudentAsync(int studentId, CancellationToken ct) =>
        await dbContext.Grades.AsNoTracking().Where(g => g.StudentId == studentId)
            .OrderByDescending(g => g.UpdatedAt ?? g.CreatedAt).Join(dbContext.Assessments, g => g.AssessmentId,
                a => a.Id,
                (g, a) => new GetGradesForStudentQueryDto(Id: g.Id, StudentId: g.StudentId,
                    AssessmentId: g.AssessmentId, AssessmentName: a.Name, Score: g.Score, Status: g.Status.ToString(),
                    UpdatedAt: g.UpdatedAt ?? g.CreatedAt)).ToListAsync(ct);
    
    public async Task<Grade?> FindStudentAssessmentGradeAsync(int studentId, int assessmentId, CancellationToken ct) =>
        await dbContext.Grades.FirstOrDefaultAsync(g => g.StudentId == studentId && g.AssessmentId == assessmentId, ct);

    public void Add(Grade grade) => dbContext.Grades.Add(grade);
}