using SchoolPortal.SharedKernel;

namespace SchoolPortal.Students.Domain.Enrollments;

public sealed class Enrollment : BaseEntity
{
    public int StudentId { get; private set; }
    public int CourseId { get; private set; }
    public int AcademicYearId { get; private set; }

    public DateOnly EnrollmentDate { get; private set; }
    public EnrollmentStatus Status { get; private set; }

    private Enrollment()
    {
    }

    private Enrollment(
        int studentId,
        int courseId,
        int academicYearId,
        DateOnly enrollmentDate)
    {
        StudentId = studentId;
        CourseId = courseId;
        AcademicYearId = academicYearId;
        EnrollmentDate = enrollmentDate;
        Status = EnrollmentStatus.Active;
    }
}