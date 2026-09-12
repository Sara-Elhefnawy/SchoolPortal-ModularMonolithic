using SchoolPortal.SharedKernel;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Domain.Assessments;

public sealed class Assessment : BaseEntity
{
    public int CourseSectionId { get; private set; }

    public string Name { get; private set; } = null!;
    public AssessmentType Type { get; private set; }
    public decimal MaximumScore { get; private set; }
    // Weight is this assessment's fraction of the term grade (0 < w <= 1),
    // not a raw percentage — flag this if your grading policy actually uses
    // 0-100, the validation below assumes the fraction convention.
    public decimal Weight { get; private set; }
    // If you removed these and relied on the interceptor's CreatedAt,
    // every query that asks "which assessments are due this week"
    // or "is this term still active" would be silently answering
    // a different question than the one asked.
    public DateOnly Date { get; private set; }

    private Assessment()
    {
    }

    private Assessment(int courseSectionId, string name, AssessmentType type,
        decimal maximumScore, decimal weight, DateOnly date)
    {
        CourseSectionId = courseSectionId;
        Name = name;
        Type = type;
        MaximumScore = maximumScore;
        Weight = weight;
        Date = date;
    }

    public static Result<Assessment> Create(int courseSectionId, string name, AssessmentType type,
        decimal maximumScore, decimal weight, DateOnly date)
    {
        var validationResult = Validate(courseSectionId, name, type, maximumScore, weight, date);
        if (validationResult.IsFailure)
            return Result<Assessment>.Failure(validationResult.Error);

        return Result<Assessment>.Success(
            new Assessment(courseSectionId, name.Trim(), type, maximumScore, weight, date));
    }

    private static Result Validate(int courseSectionId, string name, AssessmentType type,
        decimal maximumScore, decimal weight, DateOnly date)
    {
        if (courseSectionId <= 0)
            return Result.Failure(AssessmentErrors.InvalidCourseSectionId);

        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            return Result.Failure(AssessmentErrors.InvalidName);

        if (!Enum.IsDefined(type))
            return Result.Failure(AssessmentErrors.InvalidType);

        if (maximumScore <= 0)
            return Result.Failure(AssessmentErrors.InvalidMaximumScore);

        if (weight is <= 0 or > 1)
            return Result.Failure(AssessmentErrors.InvalidWeight);

        if (date == default)
            return Result.Failure(AssessmentErrors.InvalidDate);

        return Result.Success();
    }
}