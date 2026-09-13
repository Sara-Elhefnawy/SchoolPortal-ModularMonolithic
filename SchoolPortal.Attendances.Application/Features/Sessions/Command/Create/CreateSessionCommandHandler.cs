using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Application.DbContexts;
using SchoolPortal.Attendances.Domain.Sessions;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Sessions.Command.Create;

public sealed class CreateSessionCommandHandler(IAttendancesDbContext dbContext)
    : IRequestHandler<CreateSessionCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateSessionCommand request, CancellationToken ct)
    {
        var exists = await dbContext.Sessions.AnyAsync(
            s => s.CourseSectionId == request.CourseSectionId && s.DayOfWeek == request.DayOfWeek &&
                 s.StartTime == request.StartTime, ct);

        if (exists)
            return Result<int>.Failure(SessionErrors.AlreadyExists);

        var sessionResult =
            Session.Create(request.CourseSectionId, request.DayOfWeek, request.StartTime, request.EndTime);
        if (sessionResult.IsFailure)
            return Result<int>.Failure(sessionResult.Error);

        dbContext.Sessions.Add(sessionResult.Value);

        await dbContext.SaveChangesAsync(ct);

        return Result<int>.Success(sessionResult.Value.Id);
    }
}