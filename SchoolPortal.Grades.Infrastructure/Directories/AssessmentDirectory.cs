using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Features.Assessments.Query.GetByCourseSection;
using SchoolPortal.Grades.Domain.Assessments;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class AssessmentDirectory(GradesDbContext dbContext) : IAssessmentDirectory
{
    public async Task<Assessment?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.Assessments.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IReadOnlyList<GetAssessmentsByCourseSectionQueryDto>> GetBySectionAsync(int courseSectionId, CancellationToken ct) =>
        await dbContext.Assessments.Where(a => a.CourseSectionId == courseSectionId).OrderBy(a => a.Date).Select(a =>
            new GetAssessmentsByCourseSectionQueryDto(a.Id, a.CourseSectionId, a.Name, a.Type.ToString(),
                a.MaximumScore, a.Weight, a.Date)).ToListAsync(ct);

    public void Add(Assessment assessment) => dbContext.Assessments.Add(assessment);
}