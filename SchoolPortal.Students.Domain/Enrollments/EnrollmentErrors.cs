using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Students.Domain.Enrollments;

public static class EnrollmentErrors
{
    public static readonly Error InvalidStudentId =
        Error.Validation("Enrollment.InvalidStudentId", "Student id must be a positive number.");

    public static readonly Error InvalidCourseSectionId =
        Error.Validation("Enrollment.InvalidCourseSectionId", "Course section id must be a positive number.");

    public static readonly Error InvalidDate =
        Error.Validation("Enrollment.InvalidDate", "Date is required.");

    public static readonly Error InvalidTransitionToWithdrawn =
        Error.Validation("Enrollment.InvalidTransitionToWithdrawn", "Only an active enrollment can be withdrawn.");

    public static readonly Error InvalidTransitionToCompleted =
        Error.Validation("Enrollment.InvalidTransitionToCompleted", "Only an active enrollment can be completed.");
}