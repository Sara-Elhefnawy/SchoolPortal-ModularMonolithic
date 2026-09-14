using MediatR;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Domain.Attendances;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Command.Update;

public sealed class UpdateAttendanceStatusCommandHandler(IAttendanceDirectory directory)
    : IRequestHandler<UpdateAttendanceStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateAttendanceStatusCommand request, CancellationToken ct)
    {
        var attendance = await directory.FindByIdAsync(request.AttendanceId, ct);
        if (attendance is null)
            return Result.Failure(AttendanceErrors.NotFound);

        var result = attendance.UpdateStatus(request.Status, request.Note);
        if (result.IsFailure)
            return result;
        
        await directory.UpdateAsync(ct);

        return Result.Success();
    }
}