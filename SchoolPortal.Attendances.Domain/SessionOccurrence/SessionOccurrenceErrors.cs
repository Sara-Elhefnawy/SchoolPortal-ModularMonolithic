using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Attendances.Domain.SessionOccurrence;

public static class SessionOccurrenceErrors
{
    public static readonly Error InvalidSessionId =
        Error.Validation("SessionOccurrence.InvalidSessionId", "Session Id must be more than 0.");

    public static readonly Error InvalidDate =
        Error.Validation("SessionOccurrence.InvalidDate", "Date is required.");
}