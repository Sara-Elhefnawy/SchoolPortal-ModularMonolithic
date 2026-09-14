using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Grades.Domain.AcademicYears;

public static class AcademicYearErrors
{
    public static readonly Error InvalidName =
        Error.Validation("AcademicYear.InvalidName", "Name is required and must not exceed 100 characters.");

    public static readonly Error InvalidDateRange =
        Error.Validation("AcademicYear.InvalidDateRange", "Start date must be before end date.");

    public static readonly Error AlreadyExists =
        Error.Validation("AcademicYear.AlreadyExists", "AcademicYear already exists.");

    public static readonly Error NotFound =
        Error.Validation("AcademicYear.NotFound", "AcademicYear is not found.");
}