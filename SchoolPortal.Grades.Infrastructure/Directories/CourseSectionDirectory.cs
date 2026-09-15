using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.CourseSections;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class CourseSectionDirectory(GradesDbContext dbContext) : ICourseSectionDirectory
{
    public async Task<CourseSection?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.CourseSections.FirstOrDefaultAsync(cs => cs.Id == id, ct);

    public async Task<bool> CourseAndTermExistsAsync(int courseId, int termId, CancellationToken ct) =>
        await dbContext.CourseSections.AnyAsync(cs => cs.CourseId == courseId && cs.TermId == termId, ct);

    public void Add(CourseSection courseSection) => dbContext.CourseSections.Add(courseSection);
}