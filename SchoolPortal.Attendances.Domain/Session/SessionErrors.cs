using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Attendances.Domain.Session;

public static class SessionErrors
{
    public static readonly Error InvalidCourseSectionId =
        Error.Validation("Session.InvalidCourseSectionId", "Course Section Id must be more than 0.");

    public static readonly Error InvalidDayOfWeek =
        Error.Validation("Session.InvalidDayOfWeek", "Day of week is invalid.");

    public static readonly Error InvalidTimeRange =
        Error.Validation("Session.InvalidTimeRange", "Start time must be before end time.");
}