using MediatR;
using SchoolPortal.Attendance.Application.Abstractions;
using SchoolPortal.Attendance.Domain.Attendances;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendance.Application.Features.Command;

public sealed class MarkAttendanceCommandHandler(IAttendanceDirectory directory) : IRequestHandler<MarkAttendanceCommand, Result>
{
    public async Task<Result> Handle(MarkAttendanceCommand cmd, CancellationToken ct)
    {
        var existing = await directory.FindAsync(cmd.StudentId, cmd.SessionOccurrenceId, ct);

        if (existing is not null)
        {
            var updateResult = existing.UpdateStatus(cmd.Status, cmd.Note);
            if (updateResult.IsFailure)
                return updateResult;

            await directory.UpdateAsync(existing, ct);
            
            return Result.Success();
        }

        var createResult = AttendanceRecord.Create(cmd.StudentId, cmd.SessionOccurrenceId, cmd.Note, cmd.Status);
        if (createResult.IsFailure)
            return createResult;
        
        await directory.AddAsync(createResult.Value, ct);
        return Result.Success();
    }
}