using Microsoft.EntityFrameworkCore;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Features.Courses.Query.GetAll;
using SchoolPortal.Grades.Domain.Courses;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

internal sealed class CourseDirectory(GradesDbContext dbContext) : ICourseDirectory
{
    public async Task<IReadOnlyList<GetAllCoursesQueryDto>> GetAllAsync(CancellationToken ct) =>
        await dbContext.Courses.AsNoTracking().OrderBy(x => x.Code)
            .Select(x => new GetAllCoursesQueryDto(x.Id, x.Name, x.Code, x.CreditHours)).ToListAsync(ct);
    
    public async Task<Course?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<bool> ExistsWithCodeAsync(string code, CancellationToken ct) =>
        await dbContext.Courses.AnyAsync(c => c.Code == code, ct);

    public void Add(Course course) => dbContext.Courses.Add(course);
}