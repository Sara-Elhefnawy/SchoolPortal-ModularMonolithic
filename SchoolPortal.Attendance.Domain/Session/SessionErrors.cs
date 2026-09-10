using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Attendance.Domain.Session;

public static class SessionErrors
{
    public static readonly Error InvalidCourseId =
        Error.Validation("Session.InvalidCourseId", "Course Id must be more than 0.");

    public static readonly Error InvalidDayOfWeek =
        Error.Validation("Session.InvalidDayOfWeek", "Day of week is invalid.");

    public static readonly Error InvalidTimeRange =
        Error.Validation("Session.InvalidTimeRange", "Start time must be before end time.");
}