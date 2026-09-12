using SchoolPortal.SharedKernel;
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

    public static Result<Attendance> Create(
        int studentId, int sessionOccurrenceId, AttendanceStatus status, string? note = null)
    {
        var validationResult = Validate(studentId, sessionOccurrenceId, status, note);
        if (validationResult.IsFailure)
            return Result<Attendance>.Failure(validationResult.Error);

        return Result<Attendance>.Success(
            new Attendance(studentId, sessionOccurrenceId, status, note?.Trim()));
    }
    
    // No state-machine here on purpose (unlike Grade's Submit/Publish/Lock) —
    // attendance is just a status a teacher can correct any time, not a
    // multi-step workflow. If you later add a rule like "locked once grades
    // for the term are published," that guard belongs here.
    public Result UpdateStatus(AttendanceStatus status, string? note = null)
    {
        var validationResult = ValidateStatusAndNote(status, note);
        if (validationResult.IsFailure)
            return validationResult;

        Status = status;
        Note = note?.Trim();
        return Result.Success();
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