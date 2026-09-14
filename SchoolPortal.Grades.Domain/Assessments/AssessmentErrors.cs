using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Grades.Domain.Assessments;

public static class AssessmentErrors
{
    public static readonly Error InvalidCourseSectionId =
        Error.Validation("Assessment.InvalidCourseSectionId", "Course section id must be a positive number.");

    public static readonly Error InvalidName =
        Error.Validation("Assessment.InvalidName", "Name is required and must be 100 characters or fewer.");

    public static readonly Error InvalidType =
        Error.Validation("Assessment.InvalidType", "Assessment type is not a valid value.");

    public static readonly Error InvalidMaximumScore =
        Error.Validation("Assessment.InvalidMaximumScore", "Maximum score must be greater than zero.");

    public static readonly Error InvalidWeight =
        Error.Validation("Assessment.InvalidWeight", "Weight must be greater than 0 and at most 1.");

    public static readonly Error InvalidDate =
        Error.Validation("Assessment.InvalidDate", "Assessment date is required.");

    public static readonly Error WeightExceedsSectionTotal =
        Error.Validation("Assessment.WeightExceedsSectionTotal", "Weight can't exceed section total.");

    public static readonly Error NotFound =
        Error.Validation("Assessment.NotFound", "Assessment is not found.");
}