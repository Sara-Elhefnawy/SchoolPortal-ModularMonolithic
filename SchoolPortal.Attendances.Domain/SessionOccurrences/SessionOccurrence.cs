using SchoolPortal.SharedKernel;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Domain.SessionOccurrences;

// "Math's Monday slot, on March 3rd"
public sealed class SessionOccurrence : BaseEntity
{
    public int SessionId { get; private set; }
    public DateOnly Date { get; private set; }

    private SessionOccurrence() { }

    private SessionOccurrence(int sessionId, DateOnly date)
    {
        SessionId = sessionId;
        Date = date;
    }

    public static Result<SessionOccurrence> Create(int sessionId, DateOnly date)
    {
        var validationResult = Validate(sessionId, date);
        if (validationResult.IsFailure)
            return Result<SessionOccurrence>.Failure(validationResult.Error);
        
        return Result<SessionOccurrence>.Success(new SessionOccurrence(sessionId, date));
    }

    private static Result Validate(int sessionId, DateOnly date)
    {
        if (sessionId <= 0)
            return Result.Failure(SessionOccurrenceErrors.InvalidSessionId);

        if (date == default)
            return Result.Failure(SessionOccurrenceErrors.InvalidDate);

        return Result.Success();
    }
}