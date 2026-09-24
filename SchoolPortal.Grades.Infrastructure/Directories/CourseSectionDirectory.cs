using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Features.CourseSections.Query.GetByTerm;
using SchoolPortal.Grades.Application.Features.CourseSections.Query.GetRankings;
using SchoolPortal.Grades.Domain.CourseSections;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class CourseSectionDirectory(GradesDbContext dbContext) : ICourseSectionDirectory
{
    public async Task<IReadOnlyList<GetCourseSectionRankingsQueryDto>> GetRankingsAsync(int courseSectionId,
        CancellationToken ct)
    {
        var assessments = await dbContext.Assessments.AsNoTracking().Where(a => a.CourseSectionId == courseSectionId)
            .ToListAsync(ct);

        var assessmentIds = assessments.Select(a => a.Id).ToList();

        // Only Published / Locked grades count toward rankings.
        var grades = await dbContext.Grades.AsNoTracking().Where(g =>
            assessmentIds.Contains(g.AssessmentId) && (g.Status == Domain.Grades.GradeStatus.Published ||
                                                       g.Status == Domain.Grades.GradeStatus.Locked)).ToListAsync(ct);

        var byStudent = grades.GroupBy(g => g.StudentId).Select(grp =>
        {
            var totalWeight = 0m;
            var weightedScore = 0m;

            foreach (var g in grp)
            {
                var a = assessments.First(x => x.Id == g.AssessmentId);
                var pct = a.MaximumScore == 0 ? 0 : g.Score / a.MaximumScore;
                weightedScore += pct * a.Weight;
                totalWeight += a.Weight;
            }

            var avg = totalWeight == 0 ? 0m : Math.Round((weightedScore / totalWeight) * 100m, 2);

            return new { StudentId = grp.Key, Average = avg };
        }).OrderByDescending(x => x.Average).ToList();

        var rankings = byStudent.Select((x, i) => new GetCourseSectionRankingsQueryDto(i + 1, x.StudentId,
            $"Student #{x.StudentId}", // replace with real name lookup
            x.Average)).ToList();

        return rankings;
    }
    
    public async Task<CourseSection?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.CourseSections.FirstOrDefaultAsync(cs => cs.Id == id, ct);

    public async Task<IReadOnlyList<GetCourseSectionsByTermQueryDto>>
        GetByTermIdAsync(int termId, CancellationToken ct) =>
        await dbContext.CourseSections.AsNoTracking().Where(s => s.TermId == termId)
            .Join(dbContext.Courses, s => s.CourseId, c => c.Id, (s, c) => new { Section = s, Course = c }).Join(
                dbContext.Terms, x => x.Section.TermId, t => t.Id,
                (x, t) => new GetCourseSectionsByTermQueryDto(x.Section.Id, x.Section.CourseId,
                    x.Course.Code + " – " + x.Course.Name + " (" + t.Name + ")", x.Section.TermId)).ToListAsync(ct);

    public async Task<bool> CourseAndTermExistsAsync(int courseId, int termId, CancellationToken ct) =>
        await dbContext.CourseSections.AnyAsync(cs => cs.CourseId == courseId && cs.TermId == termId, ct);

    public void Add(CourseSection courseSection) => dbContext.CourseSections.Add(courseSection);
}