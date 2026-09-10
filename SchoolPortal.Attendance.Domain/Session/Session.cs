using SchoolPortal.SharedKernel;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendance.Domain.Session;

// The recurring template: "Math, every Monday, 9-10am".
public sealed class Session : BaseEntity
{
    public int CourseId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    
    private Session()
    {
    }

    private Session(int courseId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        CourseId = courseId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }

    public static Result<Session> Create(int courseId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        var validationResult = Validate(courseId, dayOfWeek, startTime, endTime);
        if (validationResult.IsFailure)
            return Result<Session>.Failure(validationResult.Error);

        return Result<Session>.Success(new Session(courseId, dayOfWeek, startTime, endTime));
    }
    
    private static Result Validate(int courseId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        if (courseId <= 0)
            return Result.Failure(SessionErrors.InvalidCourseId);

        if (!Enum.IsDefined(dayOfWeek))
            return Result.Failure(SessionErrors.InvalidDayOfWeek);

        if (startTime >= endTime)
            return Result.Failure(SessionErrors.InvalidTimeRange);

        return Result.Success();
    }
}