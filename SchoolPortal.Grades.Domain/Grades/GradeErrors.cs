using SchoolPortal.SharedKernel.Results.Errors;

namespace StudentPortal.Grades.Domain.Grades;

public static class GradeErrors
{
    public static readonly Error InvalidStudentId =
        Error.Validation("Grade.InvalidStudentId", "Student Id must be more than 0.");

    public static readonly Error InvalidAssessmentId =
        Error.Validation("Grade.InvalidAssessmentId", "Assessment Id must be more than 0.");

    public static readonly Error InvalidScore =
        Error.Validation("Grade.InvalidScore", "Score must be between 0 and the assessment's maximum score.");

    public static readonly Error InvalidTransitionToSubmitted =
        Error.Validation("Grade.InvalidTransitionToSubmitted", "Only a Draft grade can be submitted.");

    public static readonly Error InvalidTransitionToPublished =
        Error.Validation("Grade.InvalidTransitionToPublished", "Only a Submitted grade can be published.");

    public static readonly Error InvalidTransitionToLocked =
        Error.Validation("Grade.InvalidTransitionToLocked", "Only a Published grade can be locked.");
}