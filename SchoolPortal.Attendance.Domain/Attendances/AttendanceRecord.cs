using SchoolPortal.SharedKernel;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendance.Domain.Attendances;

public sealed class AttendanceRecord : BaseEntity
{
    public int StudentId { get; private set; }
    public int SessionOccurrenceId  { get; private set; }
    public string? Note { get; private set; }
    public AttendanceStatus Status { get; private set; }

    private AttendanceRecord() {}

    private AttendanceRecord(int studentId, int sessionOccurrenceId, string? note, AttendanceStatus status)
    {
        StudentId = studentId;
        SessionOccurrenceId = sessionOccurrenceId;
        Note = note;
        Status = status;
    }

    public static Result<AttendanceRecord> Create(int studentId, int sessionOccurrenceId, string? note,
        AttendanceStatus status)
    {
        var validationResult = Validate(studentId, sessionOccurrenceId, note, status);
        if (validationResult.IsFailure)
            return Result<AttendanceRecord>.Failure(validationResult.Error);

        return Result<AttendanceRecord>.Success(
            new AttendanceRecord(studentId, sessionOccurrenceId, string.IsNullOrWhiteSpace(note) ? null : note.Trim(), status));
    }
    
    // if you ever add the audit-log pattern here (ex GradeChangeLog),
    //  this is the place that would need to raise that event.
    public Result UpdateStatus(AttendanceStatus newStatus, string? note = null)
    {
        if (newStatus == AttendanceStatus.Excused && string.IsNullOrWhiteSpace(note))
            return Result.Failure(AttendanceErrors.InvalidExcusedStudent);

        Status = newStatus;
        Note = note;
        
        return Result.Success();
    }

    private static Result Validate(int studentId, int sessionOccurrenceId, string? note,
        AttendanceStatus status)
    {
        if (studentId <= 0)
            return Result.Failure(AttendanceErrors.InvalidStudentId);

        if (sessionOccurrenceId <= 0)
            return Result.Failure(AttendanceErrors.InvalidSessionOccurrenceId);

        if (!Enum.IsDefined(status))
            return Result.Failure(AttendanceErrors.InvalidStatus);

        if (status == AttendanceStatus.Excused && string.IsNullOrWhiteSpace(note))
            return Result.Failure(AttendanceErrors.InvalidExcusedStudent);

        return Result.Success();
    }
}