using SchoolPortal.SharedKernel.Domain;
using SchoolPortal.SharedKernel.Domain.Events.Attendances;
using SchoolPortal.SharedKernel.Domain.Events.Attendances.SessionOccurrences;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Domain.SessionOccurrences;

// "Math's Monday slot, on March 3rd"
public sealed class SessionOccurrence : BaseEntity
{
    public int SessionId { get; private set; }
    public DateOnly Date { get; private set; }
    public bool IsCancelled { get; private set; }

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
    
    public Result Cancel()
    {
        if (IsCancelled)
            return Result.Failure(SessionOccurrenceErrors.InvalidTransitionToCancelled);

        IsCancelled = true;

        RaiseEvent(() => new SessionOccurrenceCancelled(Id, SessionId, Date));
        
        return Result.Success();
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