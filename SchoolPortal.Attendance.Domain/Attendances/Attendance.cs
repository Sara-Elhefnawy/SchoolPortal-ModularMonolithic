using SchoolPortal.Attendance.Domain.Attendances;
using SchoolPortal.SharedKernel;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendance.Domain.Attendances;

public sealed class Attendance : BaseEntity
{
    public int StudentId { get; private set; }
    public int SessionOccurrenceId  { get; private set; }
    public string? Note { get; private set; } = string.Empty;
    public AttendanceStatus Status { get; private set; }
    public DateTime RecordedAt { get; private set; }

    private Attendance() {}

    private Attendance(int studentId, int sessionOccurrenceId, string? note, AttendanceStatus status, DateTime recordedAt)
    {
        StudentId = studentId;
        SessionOccurrenceId = sessionOccurrenceId;
        Note = note;
        Status = status;
        RecordedAt = recordedAt;
    }

    public static Result<Attendance> Create(int studentId, int sessionOccurrenceId, string? note,
        AttendanceStatus status, DateTime recordedAt)
    {
        var validationResult = Validate(studentId, sessionOccurrenceId, note, status, recordedAt);
        if (validationResult.IsFailure)
            return Result<Attendance>.Failure(validationResult.Error);

        return Result<Attendance>.Success(
            new Attendance(studentId, sessionOccurrenceId, string.IsNullOrWhiteSpace(note) ? null : note.Trim(), status, recordedAt));
    }

    private static Result Validate(int studentId, int sessionOccurrenceId, string? note,
        AttendanceStatus status, DateTime recordedAt)
    {
        if (studentId <= 0)
            return Result.Failure(AttendanceErrors.InvalidStudentId);

        if (sessionOccurrenceId <= 0)
            return Result.Failure(AttendanceErrors.InvalidSessionOccurrenceId);

        if (!Enum.IsDefined(status))
            return Result.Failure(AttendanceErrors.InvalidStatus);

        if (status == AttendanceStatus.Excused && string.IsNullOrWhiteSpace(note))
            return Result.Failure(AttendanceErrors.InvalidExcusedStudent);

        if (recordedAt == default)
            return Result.Failure(AttendanceErrors.InvalidDate);

        return Result.Success();
    }
}