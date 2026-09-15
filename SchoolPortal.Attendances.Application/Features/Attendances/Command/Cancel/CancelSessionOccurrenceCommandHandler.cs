using MediatR;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Application.Persistence;
using SchoolPortal.Attendances.Domain.SessionOccurrences;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Command.Cancel;

public sealed class CancelSessionOccurrenceCommandHandler(
    ISessionOccurrenceDirectory directory,
    IAttendancesUnitOfWork unitOfWork) 
    : IRequestHandler<CancelSessionOccurrenceCommand, Result>
{
    public async Task<Result> Handle(CancelSessionOccurrenceCommand request, CancellationToken ct)
    {
        var occurrence = await directory.GetByIdAsync(request.Id, ct);

        if (occurrence is null)
            return Result.Failure(SessionOccurrenceErrors.NotFound);

        var result = occurrence.Cancel();

        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}