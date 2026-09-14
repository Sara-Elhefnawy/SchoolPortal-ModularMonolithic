using Microsoft.EntityFrameworkCore;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Domain.Enrollments;
using SchoolPortal.Students.Infrastructure.Persistence;

namespace SchoolPortal.Students.Infrastructure.Directories;

public sealed class EnrollmentDirectory(StudentsDbContext dbContext) : IEnrollmentDirectory
{
    public async Task<Enrollment?> FindByIdAsync(int enrollmentId, CancellationToken ct) =>
        await dbContext.Enrollments.FirstOrDefaultAsync(a => a.Id == enrollmentId, ct);

    public async Task<bool> ExistsAsync(int studentId, int courseSectionId, CancellationToken ct) =>
        await dbContext.Enrollments.AnyAsync(a => a.StudentId == studentId && a.CourseSectionId == courseSectionId, ct);

    public async Task AddAsync(Enrollment enrollment, CancellationToken ct)
    {
        dbContext.Enrollments.Add(enrollment);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(CancellationToken ct) => await dbContext.SaveChangesAsync(ct); // already tracked
}