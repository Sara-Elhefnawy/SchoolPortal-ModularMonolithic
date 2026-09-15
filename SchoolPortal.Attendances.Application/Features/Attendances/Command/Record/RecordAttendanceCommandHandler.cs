using MediatR;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Application.Persistence;
using SchoolPortal.Attendances.Domain.Attendances;
using SchoolPortal.Attendances.Domain.SessionOccurrences;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Command.Record;

public sealed class RecordAttendanceCommandHandler(
    IAttendanceDirectory directory, 
    ISessionOccurrenceDirectory occurrenceDirectory,
    IAttendancesUnitOfWork unitOfWork)
    : IRequestHandler<RecordAttendanceCommand, Result<int>>
{
    public async Task<Result<int>> Handle(RecordAttendanceCommand request, CancellationToken ct)
    {
        // StudentId can't be verified here — cross-module, no FK, no directory call.

        var occurrenceExists = await occurrenceDirectory.ExistsByIdAsync(request.SessionOccurrenceId, ct);
        if (!occurrenceExists)
            return Result<int>.Failure(SessionOccurrenceErrors.NotFound);

        var alreadyRecorded = await directory.FindStudentAndSessionOccurrenceAsync(request.StudentId, request.SessionOccurrenceId, ct);
        if (alreadyRecorded is not null)
            return Result<int>.Failure(AttendanceErrors.AlreadyRecorded); // direct them to UpdateAttendanceStatus instead

        var attendanceResult =
            Attendance.Create(request.StudentId, request.SessionOccurrenceId, request.Status, request.Note);
        if (attendanceResult.IsFailure)
            return Result<int>.Failure(attendanceResult.Error);

        directory.Add(attendanceResult.Value);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<int>.Success(attendanceResult.Value.Id);
    }
}