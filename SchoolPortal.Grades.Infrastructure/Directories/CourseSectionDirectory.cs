using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.CourseSections;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class CourseSectionDirectory(GradesDbContext db) : ICourseSectionDirectory
{
    public Task<CourseSection?> FindAsync(int id, CancellationToken ct)
        => db.CourseSections.FirstOrDefaultAsync(cs => cs.Id == id, ct);

    public Task<bool> ExistsAsync(int courseId, int termId, CancellationToken ct)
        => db.CourseSections.AnyAsync(cs => cs.CourseId == courseId && cs.TermId == termId, ct);

    public Task AddAsync(CourseSection courseSection, CancellationToken ct)
    {
        db.CourseSections.Add(courseSection);
        return db.SaveChangesAsync(ct);
    }
}