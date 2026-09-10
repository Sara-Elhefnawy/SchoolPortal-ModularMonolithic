using SchoolPortal.SharedKernel;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.AcademicYear.Domain.AcademicYear;

public sealed class AcademicYear : BaseEntity
{
    public string Name { get; private set; } = null!;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public bool IsCurrent { get; private set; }

    private AcademicYear()
    {
    }

    private AcademicYear(
        string name,
        DateOnly startDate,
        DateOnly endDate)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        // A new academic year is never current by default — "current" is a
        // deliberate admin action (see MarkAsCurrent), not a side effect of creation.
        IsCurrent = false;
    }

    public static Result<AcademicYear> Create(string name, DateOnly startDate, DateOnly endDate)
    {
        var validationResult = Validate(name, startDate, endDate);
        if (validationResult.IsFailure)
            return Result<AcademicYear>.Failure(validationResult.Error);

        return Result<AcademicYear>.Success(new AcademicYear(name.Trim(), startDate, endDate));
    }

    // Explicit behavior instead of a public setter: "becoming current" is a business
    // event (only one year should be current at a time), so the handler that flips
    // this should also be the one responsible for un-marking the previous year —
    // that coordination belongs in the Application layer, not here.
    public Result MarkAsCurrent()
    {
        IsCurrent = true;
        return Result.Success();
    }

    private static Result Validate(string name, DateOnly startDate, DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            return Result.Failure(AcademicYearErrors.InvalidName);

        if (startDate >= endDate)
            return Result.Failure(AcademicYearErrors.InvalidDateRange);

        return Result.Success();
    }
}