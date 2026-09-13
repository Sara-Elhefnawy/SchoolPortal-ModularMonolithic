using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Application.DbContexts;
using SchoolPortal.Attendances.Domain.Sessions;
using SchoolPortal.Attendances.Domain.SessionOccurrences;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.SessionOccurrences.Command.Create;

public sealed class CreateSessionOccurrenceCommandHandler(IAttendancesDbContext db)
    : IRequestHandler<CreateSessionOccurrenceCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateSessionOccurrenceCommand request, CancellationToken ct)
    {
        var sessionExists = await db.Sessions.AnyAsync(s => s.Id == request.SessionId, ct);
        if (!sessionExists)
            return Result<int>.Failure(SessionErrors.NotFound);

        var sessionOccurrenceExists = await db.SessionOccurrences.AnyAsync(
            o => o.SessionId == request.SessionId && o.Date == request.Date, ct);
        if (sessionOccurrenceExists)
            return Result<int>.Failure(SessionOccurrenceErrors.AlreadyExists);

        var occurrenceResult = SessionOccurrence.Create(request.SessionId, request.Date);
        if (occurrenceResult.IsFailure)
            return Result<int>.Failure(occurrenceResult.Error);

        db.SessionOccurrences.Add(occurrenceResult.Value);
        await db.SaveChangesAsync(ct);

        return Result<int>.Success(occurrenceResult.Value.Id);
    }
}