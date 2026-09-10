using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.AcademicYear.Domain.AcademicYear;

public static class AcademicYearErrors
{
    public static readonly Error InvalidName =
        Error.Validation("AcademicYear.InvalidName", "Name is required and must not exceed 100 characters.");

    public static readonly Error InvalidDateRange =
        Error.Validation("AcademicYear.InvalidDateRange", "Start date must be before end date.");
}