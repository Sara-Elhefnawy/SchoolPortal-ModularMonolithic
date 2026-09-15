using MediatR;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Application.Persistence;
using SchoolPortal.Attendances.Domain.Attendances;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Command.Update;

public sealed class UpdateAttendanceStatusCommandHandler(
    IAttendanceDirectory directory,
    IAttendancesUnitOfWork unitOfWork)
    : IRequestHandler<UpdateAttendanceStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateAttendanceStatusCommand request, CancellationToken ct)
    {
        var attendance = await directory.GetByIdAsync(request.AttendanceId, ct);
        if (attendance is null)
            return Result.Failure(AttendanceErrors.NotFound);

        var result = attendance.UpdateStatus(request.Status, request.Note);
        if (result.IsFailure)
            return result;
        
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}