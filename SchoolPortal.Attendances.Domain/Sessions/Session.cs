using SchoolPortal.SharedKernel;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Domain.Sessions;

// The recurring template: "Math, every Monday, 9-10am" — for one specific
// CourseSection, so re-running the same course next year creates a new
// CourseSection and a new set of Sessions, rather than silently reusing
// last year's Monday 9am slot.
public sealed class Session : BaseEntity
{
    public int CourseSectionId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    
    private Session()
    {
    }

    private Session(int courseSectionId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        CourseSectionId = courseSectionId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }

    public static Result<Session> Create(int courseSectionId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        var validationResult = Validate(courseSectionId, dayOfWeek, startTime, endTime);
        if (validationResult.IsFailure)
            return Result<Session>.Failure(validationResult.Error);

        return Result<Session>.Success(new Session(courseSectionId, dayOfWeek, startTime, endTime));
    }
    
    private static Result Validate(int courseSectionId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        if (courseSectionId <= 0)
            return Result.Failure(SessionErrors.InvalidCourseSectionId);

        if (!Enum.IsDefined(dayOfWeek))
            return Result.Failure(SessionErrors.InvalidDayOfWeek);

        if (startTime >= endTime)
            return Result.Failure(SessionErrors.InvalidTimeRange);

        return Result.Success();
    }
}