using MediatR;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Application.Persistence;
using SchoolPortal.Attendances.Domain.Sessions;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Sessions.Command.Create;

public sealed class CreateSessionCommandHandler(
    ISessionDirectory  directory,
    IAttendancesUnitOfWork unitOfWork)
    : IRequestHandler<CreateSessionCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateSessionCommand request, CancellationToken ct)
    {
        var exists = await directory.ConflictExistsAsync(request.CourseSectionId, request.DayOfWeek, request.StartTime, ct);
        if (exists)
            return Result<int>.Failure(SessionErrors.AlreadyExists);

        var sessionResult = Session.Create(request.CourseSectionId, request.DayOfWeek, request.StartTime, request.EndTime);
        if (sessionResult.IsFailure)
            return Result<int>.Failure(sessionResult.Error);

        directory.Add(sessionResult.Value);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<int>.Success(sessionResult.Value.Id);
    }
}