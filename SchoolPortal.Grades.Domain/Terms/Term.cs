using SchoolPortal.SharedKernel;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Domain.Terms;

public sealed class Term : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public TermType Type { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public int AcademicYearId { get; private set; }

    private Term()
    {
    }

    private Term(
        string name,
        TermType type,
        DateOnly startDate,
        DateOnly endDate,
        int academicYearId)
    {
        Name = name;
        Type = type;
        StartDate = startDate;
        EndDate = endDate;
        AcademicYearId = academicYearId;
    }
    
    public static Result<Term> Create(string name, TermType type, DateOnly startDate, DateOnly endDate, int academicYearId)
    {
        var validationResult = Validate(name, type, startDate, endDate, academicYearId);
        if (validationResult.IsFailure)
            return Result<Term>.Failure(validationResult.Error);

        return Result<Term>.Success(new Term(name.Trim(), type, startDate, endDate, academicYearId));
    }

    private static Result Validate(string name, TermType type, DateOnly startDate, DateOnly endDate, int academicYearId)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            return Result.Failure(TermErrors.InvalidName);

        if (!Enum.IsDefined(type))
            return Result.Failure(TermErrors.InvalidType);

        if (startDate >= endDate)
            return Result.Failure(TermErrors.InvalidDateRange);

        if (academicYearId <= 0)
            return Result.Failure(TermErrors.InvalidAcademicYearId);

        return Result.Success();
    }
}