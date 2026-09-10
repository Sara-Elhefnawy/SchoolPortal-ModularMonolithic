using SchoolPortal.SharedKernel;
using SchoolPortal.SharedKernel.Results;

namespace StudentPortal.Grades.Domain.Grades;

public class Grade : BaseEntity
{
    public int StudentId { get; private set; }
    public int AssessmentId { get; private set; }
    public decimal Score { get; private set; }
    public GradeStatus Status { get; private set; }
    
    private Grade()
    {
    }

    private Grade(
        int studentId,
        int assessmentId,
        decimal score)
    {
        StudentId = studentId;
        AssessmentId = assessmentId;
        Score = score;
        // Starts as Draft, not Published — a freshly entered grade hasn't gone
        // through the submit/publish workflow your GradeStatus enum implies yet.
        Status = GradeStatus.Draft;
    }

    // assessmentMaximumScore is passed in (not looked up here) because the Domain
    // layer shouldn't reach out to a repository — the handler loads the Assessment
    // and passes its MaximumScore in, keeping this a pure, testable validation.
    public static Result<Grade> Create(int studentId, int assessmentId, decimal score, decimal assessmentMaximumScore)
    {
        var validationResult = Validate(studentId, assessmentId, score, assessmentMaximumScore);
        if (validationResult.IsFailure)
            return Result<Grade>.Failure(validationResult.Error);
        
        return Result<Grade>.Success(new Grade(studentId, assessmentId, score));
    }

    // State-transition methods instead of a public Status setter — this stops
    // a handler from ever jumping straight from Draft to Locked by mistake.
    public Result Submit()
    {
        if (Status != GradeStatus.Draft)
            return Result.Failure(GradeErrors.InvalidTransitionToSubmitted);

        Status = GradeStatus.Submitted;
        return Result.Success();
    }

    public Result Publish()
    {
        if (Status != GradeStatus.Submitted)
            return Result.Failure(GradeErrors.InvalidTransitionToPublished);

        Status = GradeStatus.Published;
        return Result.Success();
    }

    public Result Lock()
    {
        if (Status != GradeStatus.Published)
            return Result.Failure(GradeErrors.InvalidTransitionToLocked);

        Status = GradeStatus.Locked;
        return Result.Success();
    }

    private static Result Validate(int studentId, int assessmentId, decimal score, decimal assessmentMaximumScore)
    {
        if (studentId <= 0)
            return Result.Failure(GradeErrors.InvalidStudentId);

        if (assessmentId <= 0)
            return Result.Failure(GradeErrors.InvalidAssessmentId);

        if (score < 0 || score > assessmentMaximumScore)
            return Result.Failure(GradeErrors.InvalidScore);

        return Result.Success();
    }
}