using MediatR;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Application.Persistence;
using SchoolPortal.Attendances.Domain.Sessions;
using SchoolPortal.Attendances.Domain.SessionOccurrences;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.SessionOccurrences.Command.Create;

public sealed class CreateSessionOccurrenceCommandHandler(
    ISessionOccurrenceDirectory directory, 
    ISessionDirectory sessionDirectory,
    IAttendancesUnitOfWork unitOfWork)
    : IRequestHandler<CreateSessionOccurrenceCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateSessionOccurrenceCommand request, CancellationToken ct)
    {
        var sessionExists = await sessionDirectory.ExistsAsync(request.SessionId, ct);
        if (!sessionExists)
            return Result<int>.Failure(SessionErrors.NotFound);

        var sessionOccurrenceExists = await directory.ExistsForDateAsync(request.SessionId, request.Date, ct);
        if (sessionOccurrenceExists)
            return Result<int>.Failure(SessionOccurrenceErrors.AlreadyExists);

        var occurrenceResult = SessionOccurrence.Create(request.SessionId, request.Date);
        if (occurrenceResult.IsFailure)
            return Result<int>.Failure(occurrenceResult.Error);
        
        directory.Add(occurrenceResult.Value);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<int>.Success(occurrenceResult.Value.Id);
    }
}