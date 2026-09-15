using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Attendances.Domain.SessionOccurrences;

public static class SessionOccurrenceErrors
{
    public static readonly Error InvalidSessionId =
        Error.Validation("SessionOccurrence.InvalidSessionId", "Session Id must be more than 0.");

    public static readonly Error InvalidDate =
        Error.Validation("SessionOccurrence.InvalidDate", "Date is required.");

    public static readonly Error InvalidTransitionToCancelled =
        Error.Validation("SessionOccurrence.InvalidTransitionToCancelled", "Session occurrence is already cancelled.");
    
    public static readonly Error AlreadyExists = Error.Validation("SessionOccurrence.AlreadyExists",
        "Section Occurrence already exists with the same course section id and start time and day of week.");
    
    public static readonly Error NotFound = Error.Validation("SessionOccurrence.NotFound",
        "Section Occurrence not found.");
}