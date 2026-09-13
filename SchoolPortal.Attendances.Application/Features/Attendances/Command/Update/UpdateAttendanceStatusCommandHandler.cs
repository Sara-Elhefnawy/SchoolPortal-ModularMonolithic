using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Application.DbContexts;
using SchoolPortal.Attendances.Domain.Attendances;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Command.Update;

public sealed class UpdateAttendanceStatusCommandHandler(IAttendancesDbContext db)
    : IRequestHandler<UpdateAttendanceStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateAttendanceStatusCommand request, CancellationToken ct)
    {
        var attendance = await db.Attendances.FirstOrDefaultAsync(a => a.Id == request.AttendanceId, ct);
        if (attendance is null)
            return Result.Failure(AttendanceErrors.NotFound);

        var result = attendance.UpdateStatus(request.Status, request.Note);
        if (result.IsFailure)
            return result;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}