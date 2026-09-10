using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Assessment.Domain.Assessment;

public static class AssessmentErrors
{
    public static readonly Error InvalidCourseId =
        Error.Validation("Assessment.InvalidCourseId", "Course Id must be more than 0.");

    public static readonly Error InvalidTermId =
        Error.Validation("Assessment.InvalidTermId", "Term Id must be more than 0.");

    public static readonly Error InvalidName =
        Error.Validation("Assessment.InvalidName", "Name is required and must not exceed 100 characters.");

    public static readonly Error InvalidType =
        Error.Validation("Assessment.InvalidType", "Assessment type is invalid.");

    public static readonly Error InvalidMaximumScore =
        Error.Validation("Assessment.InvalidMaximumScore", "Maximum score must be greater than 0.");

    public static readonly Error InvalidWeight =
        Error.Validation("Assessment.InvalidWeight", "Weight must be a fraction greater than 0 and no more than 1.");

    public static readonly Error InvalidDate =
        Error.Validation("Assessment.InvalidDate", "Date is required.");
}