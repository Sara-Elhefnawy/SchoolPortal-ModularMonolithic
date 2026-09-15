using SchoolPortal.Students.Domain.Enrollments;

namespace SchoolPortal.Students.Application.Directories;

public interface IEnrollmentDirectory
{
    Task<Enrollment?> GetByIdAsync(int enrollmentId, CancellationToken ct);
    Task<bool> StudentAndCourseSectionExistsAsync(int studentId, int courseSectionId, CancellationToken ct);
    void Add(Enrollment enrollment);
}