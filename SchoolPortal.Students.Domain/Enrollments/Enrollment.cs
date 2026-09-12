using SchoolPortal.SharedKernel;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Students.Domain.Enrollments;

public sealed class Enrollment : BaseEntity
{
    public int StudentId { get; private set; }
    public int CourseSectionId { get; private set; }
    // the date this student joined a specific course section
    public DateOnly Date { get; private set; }
    public EnrollmentStatus Status { get; private set; }

    private Enrollment()
    {
    }

    private Enrollment(int studentId, int courseSectionId, DateOnly date)
    {
        StudentId = studentId;
        CourseSectionId = courseSectionId;
        Date = date;
        Status = EnrollmentStatus.Active;
    }

    public static Result<Enrollment> Create(int studentId, int courseSectionId, DateOnly date)
    {
        var validationResult = Validate(studentId, courseSectionId, date);
        if (validationResult.IsFailure)
            return Result<Enrollment>.Failure(validationResult.Error);

        return Result<Enrollment>.Success(new Enrollment(studentId, courseSectionId, date));
    }

    public Result Withdraw()
    {
        if (Status != EnrollmentStatus.Active)
            return Result.Failure(EnrollmentErrors.InvalidTransitionToWithdrawn);

        Status = EnrollmentStatus.Withdrawn;
        return Result.Success();
    }

    public Result Complete()
    {
        if (Status != EnrollmentStatus.Active)
            return Result.Failure(EnrollmentErrors.InvalidTransitionToCompleted);

        Status = EnrollmentStatus.Completed;
        return Result.Success();
    }

    private static Result Validate(int studentId, int courseSectionId, DateOnly date)
    {
        if (studentId <= 0)
            return Result.Failure(EnrollmentErrors.InvalidStudentId);

        if (courseSectionId <= 0)
            return Result.Failure(EnrollmentErrors.InvalidCourseSectionId);

        if (date == default)
            return Result.Failure(EnrollmentErrors.InvalidDate);

        return Result.Success();
    }
}