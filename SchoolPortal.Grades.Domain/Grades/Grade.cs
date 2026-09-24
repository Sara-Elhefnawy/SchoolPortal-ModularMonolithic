using SchoolPortal.SharedKernel.Domain;
using SchoolPortal.SharedKernel.Domain.Events.Grades;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Domain.Grades;

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

        var grade = new Grade(studentId, assessmentId, score);
        
        grade.RaiseEvent(() => new GradeCreated(grade.StudentId, grade.AssessmentId, grade.Score));
        
        return Result<Grade>.Success(grade);
    }

    // Draft --Submit()--> Submitted --Publish()--> Published --Lock()--> Locked
    
    // Draft -> Submitted.
    // Nothing else can move into Submitted directly
    public Result Submit()
    {
        if (Status != GradeStatus.Draft)
            return Result.Failure(GradeErrors.InvalidTransitionToSubmitted);
        
        Status = GradeStatus.Submitted;
        
        RaiseEvent(() => new GradeSubmitted(Id, StudentId));
        
        return Result.Success();
    }
 
    // Submitted -> Draft.
    // The one backward move in the flow:
    // lets a teacher pull a grade back for correction before it's published
    public Result Draft()
    {
        if (Status != GradeStatus.Submitted)
            return Result.Failure(GradeErrors.InvalidTransitionToDraft);
 
        Status = GradeStatus.Draft;
 
        RaiseEvent(() => new GradeDrafted(Id, StudentId));
 
        return Result.Success();
    }
 
    // Submitted -> Published.
    public Result Publish()
    {
        if (Status != GradeStatus.Submitted)
            return Result.Failure(GradeErrors.InvalidTransitionToPublished);
 
        Status = GradeStatus.Published;
 
        RaiseEvent(() => new GradePublished(Id, StudentId));
 
        return Result.Success();
    }
 
    // Published -> Locked.
    public Result Lock()
    {
        if (Status != GradeStatus.Published)
            return Result.Failure(GradeErrors.InvalidTransitionToLocked);
 
        Status = GradeStatus.Locked;
 
        RaiseEvent(() => new GradeLocked(Id, StudentId));
 
        return Result.Success();
    }
    
    // Locked -> Draft,
    // bypassing the normal "nothing moves out of Locked" rule.
    public Result OverrideUnlock()
    {
        if (Status != GradeStatus.Locked)
            return Result.Failure(GradeErrors.InvalidTransitionToUnlock);
 
        Status = GradeStatus.Draft;
 
        RaiseEvent(() => new GradeUnlocked(Id, StudentId));
 
        return Result.Success();
    }
    
    public Result ReviseScore(decimal newScore)
    {
        if (Status is GradeStatus.Published or GradeStatus.Locked)
            return Result.Failure(GradeErrors.InvalidRevise);

        var oldScore = Score;
        Score = newScore;

        if (oldScore != newScore)
            RaiseEvent(() => new GradeChanged(Id, StudentId, oldScore, newScore));

        return Result.Success();
    }
    
    public Result Unpublish()
    {
        if (Status != GradeStatus.Published)
            return Result.Failure(GradeErrors.InvalidTransitionToUnpublish);

        Status = GradeStatus.Draft;

        RaiseEvent(() => new GradeUnpublished(Id, StudentId));
        
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