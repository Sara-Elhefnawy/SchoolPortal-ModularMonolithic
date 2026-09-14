using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Grades.Domain.Terms;

public static class TermErrors
{
    public static readonly Error InvalidName =
        Error.Validation("Term.InvalidName", "Name is required and must not exceed 100 characters.");

    public static readonly Error InvalidType =
        Error.Validation("Term.InvalidType", "Term type is invalid.");

    public static readonly Error InvalidDateRange =
        Error.Validation("Term.InvalidDateRange", "Start date must be before end date.");

    public static readonly Error InvalidAcademicYearId =
        Error.Validation("Term.InvalidAcademicYearId", "Academic Year Id must be more than 0.");

    public static readonly Error AlreadyExists =
        Error.Validation("Term.AlreadyExists", "Term already exists with this name and same academic year id.");
    
    public static readonly Error NotFound =
        Error.Validation("Term.NotFound", "Term is not found.");
}