using SchoolPortal.SharedKernel;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Assessment.Domain.Assessment;

public sealed class Assessment : BaseEntity
{
    public int CourseId { get; private set; }
    public int TermId { get; private set; }

    public string Name { get; private set; } = null!;
    public AssessmentType Type { get; private set; }
    public decimal MaximumScore { get; private set; }
    // Weight is treated as this assessment's fraction of the term grade (0 < w <= 1),
    // not a raw percentage — flag this if your grading policy actually uses 0-100,
    // the validation below assumes the fraction convention.
    public decimal Weight { get; private set; }    
    public DateOnly Date { get; private set; }

    private Assessment()
    {
    }

    private Assessment(int courseId, int termId, string name, AssessmentType type,
        decimal maximumScore, decimal weight, DateOnly date)
    {
        CourseId = courseId;
        TermId = termId;
        Name = name;
        Type = type;
        MaximumScore = maximumScore;
        Weight = weight;
        Date = date;
    }

    public static Result<Assessment> Create(int courseId, int termId, string name, AssessmentType type,
        decimal maximumScore, decimal weight, DateOnly date)
    {
        var validationResult = Validate(courseId, termId, name, type, maximumScore, weight, date);
        if (validationResult.IsFailure)
            return Result<Assessment>.Failure(validationResult.Error);

        return Result<Assessment>.Success(
            new Assessment(courseId, termId, name.Trim(), type, maximumScore, weight, date));
    }

    private static Result Validate(int courseId, int termId, string name, AssessmentType type,
        decimal maximumScore, decimal weight, DateOnly date)
    {
        if (courseId <= 0)
            return Result.Failure(AssessmentErrors.InvalidCourseId);

        if (termId <= 0)
            return Result.Failure(AssessmentErrors.InvalidTermId);

        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            return Result.Failure(AssessmentErrors.InvalidName);

        if (!Enum.IsDefined(type))
            return Result.Failure(AssessmentErrors.InvalidType);

        if (maximumScore <= 0)
            return Result.Failure(AssessmentErrors.InvalidMaximumScore);

        if (weight <= 0 || weight > 1)
            return Result.Failure(AssessmentErrors.InvalidWeight);

        if (date == default)
            return Result.Failure(AssessmentErrors.InvalidDate);

        return Result.Success();
    }
}