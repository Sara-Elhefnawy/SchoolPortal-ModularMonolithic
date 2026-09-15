using SchoolPortal.SharedKernel.Domain;
using SchoolPortal.SharedKernel.Domain.Events.Attendances;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Domain.Attendances;

// Ties one student to one concrete occurrence of a session.
// Session -> SessionOccurrence only tells you
// "the Monday 9am slot happened on March 3rd" — this is what says who was actually there.
public sealed class Attendance : BaseEntity
{
    public int StudentId { get; private set; }
    public int SessionOccurrenceId { get; private set; }
    public AttendanceStatus Status { get; private set; }
    public string? Note { get; private set; }

    private Attendance()
    {
    }

    private Attendance(int studentId, int sessionOccurrenceId, AttendanceStatus status, string? note)
    {
        StudentId = studentId;
        SessionOccurrenceId = sessionOccurrenceId;
        Status = status;
        Note = note;
    }

    public static Result<Attendance> Create(int studentId, int sessionOccurrenceId, AttendanceStatus status,
        string? note = null)
    {
        var validationResult = Validate(studentId, sessionOccurrenceId, status, note);

        if (validationResult.IsFailure)
            return Result<Attendance>.Failure(validationResult.Error);

        var attendance = new Attendance(studentId, sessionOccurrenceId, status, note?.Trim());

        attendance.RaiseEvent(() => CreateStatusEvent(attendance));

        return Result<Attendance>.Success(attendance);
    }

    public Result UpdateStatus(AttendanceStatus status, string? note = null)
    {
        var validationResult = ValidateStatusAndNote(status, note);

        if (validationResult.IsFailure)
            return validationResult;

        if (Status == status && Note == note?.Trim())
            return Result.Success();

        Status = status;
        Note = note?.Trim();

        RaiseEvent(() => CreateStatusEvent(this));

        return Result.Success();
    }

    // No state-machine here on purpose (unlike Grade's Submit/Publish/Lock) —
    // attendance is just a status a teacher can correct any time, not a
    // multi-step workflow. If you later add a rule like "locked once grades
    // for the term are published," that guard belongs here.
    private static IDomainEvent CreateStatusEvent(Attendance attendance)
    {
        return attendance.Status switch
        {
            AttendanceStatus.Present => new AttendanceMarkedPresent(attendance.Id, attendance.StudentId,
                attendance.SessionOccurrenceId),

            AttendanceStatus.Absent => new AttendanceMarkedAbsent(attendance.Id, attendance.StudentId,
                attendance.SessionOccurrenceId),

            AttendanceStatus.Late => new AttendanceMarkedLate(attendance.Id, attendance.StudentId,
                attendance.SessionOccurrenceId),

            AttendanceStatus.Excused => new AttendanceMarkedExcused(attendance.Id, attendance.StudentId,
                attendance.SessionOccurrenceId),

            AttendanceStatus.EarlyLeave => new AttendanceMarkedEarlyLeave(attendance.Id, attendance.StudentId,
                attendance.SessionOccurrenceId),

            _ => throw new InvalidOperationException($"Unhandled attendance status: {attendance.Status}")
        };
    }

    private static Result Validate(int studentId, int sessionOccurrenceId, AttendanceStatus status, string? note)
    {
        if (studentId <= 0)
            return Result.Failure(AttendanceErrors.InvalidStudentId);

        if (sessionOccurrenceId <= 0)
            return Result.Failure(AttendanceErrors.InvalidSessionOccurrenceId);

        return ValidateStatusAndNote(status, note);
    }

    private static Result ValidateStatusAndNote(AttendanceStatus status, string? note)
    {
        if (!Enum.IsDefined(status))
            return Result.Failure(AttendanceErrors.InvalidStatus);

        if (note is { Length: > 250 })
            return Result.Failure(AttendanceErrors.InvalidNote);

        return Result.Success();
    }
}