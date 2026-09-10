using SchoolPortal.SharedKernel;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Domain.Course;

public sealed class Course : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public decimal CreditHours { get; private set; }

    private Course()
    {
    }

    private Course(
        string name,
        string code,
        decimal creditHours)
    {
        Name = name;
        Code = code;
        CreditHours = creditHours;
    }
    
    public static Result<Course> Create(string name, string code, decimal creditHours)
    {
        var validationResult = Validate(name, code, creditHours);
        if (validationResult.IsFailure)
            return Result<Course>.Failure(validationResult.Error);

        return Result<Course>.Success(new Course(name.Trim(), code.Trim().ToUpperInvariant(), creditHours));
    }

    private static Result Validate(string name, string code, decimal creditHours)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            return Result.Failure(CourseErrors.InvalidName);

        if (string.IsNullOrWhiteSpace(code) || code.Trim().Length > 20)
            return Result.Failure(CourseErrors.InvalidCode);

        if (creditHours <= 0)
            return Result.Failure(CourseErrors.InvalidCreditHours);

        return Result.Success();
    }
}