using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Application.DbContexts;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Domain.Attendances;
using SchoolPortal.Attendances.Domain.SessionOccurrences;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Command.Record;

public sealed class RecordAttendanceCommandHandler(IAttendancesDbContext db)
    : IRequestHandler<RecordAttendanceCommand, Result<int>>
{
    public async Task<Result<int>> Handle(RecordAttendanceCommand request, CancellationToken ct)
    {
        // StudentId can't be verified here — cross-module, no FK, no directory call.

        var occurrenceExists = await db.SessionOccurrences.AnyAsync(o => o.Id == request.SessionOccurrenceId, ct);
        if (!occurrenceExists)
            return Result<int>.Failure(SessionOccurrenceErrors.NotFound);

        var alreadyRecorded = await db.Attendances.AnyAsync(
            a => a.StudentId == request.StudentId && a.SessionOccurrenceId == request.SessionOccurrenceId, ct);
        if (alreadyRecorded)
            return Result<int>.Failure(AttendanceErrors.AlreadyRecorded); // direct them to UpdateAttendanceStatus instead

        var attendanceResult =
            Attendance.Create(request.StudentId, request.SessionOccurrenceId, request.Status, request.Note);
        if (attendanceResult.IsFailure)
            return Result<int>.Failure(attendanceResult.Error);

        db.Attendances.Add(attendanceResult.Value);
        await db.SaveChangesAsync(ct);

        return Result<int>.Success(attendanceResult.Value.Id);
    }
}