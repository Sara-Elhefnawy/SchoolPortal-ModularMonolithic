using SchoolPortal.Students.Domain.Enrollments;

namespace SchoolPortal.Students.Application.Directories;

public interface IEnrollmentDirectory
{
    Task<Enrollment?> FindByIdAsync(int enrollmentId, CancellationToken ct);
    Task<bool> ExistsAsync(int studentId, int courseSectionId, CancellationToken ct);
    Task AddAsync(Enrollment enrollment, CancellationToken ct);
    Task UpdateAsync(CancellationToken ct);
}