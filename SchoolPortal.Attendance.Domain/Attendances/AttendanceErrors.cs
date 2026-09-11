using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Attendance.Domain.Attendances;

public static class AttendanceErrors
{
    public static readonly Error InvalidExcusedStudent =
        Error.Validation("Attendance.InvalidExcusedStudent", "An excused absence requires a note explaining why.");

    public static readonly Error InvalidStudentId =
        Error.Validation("Attendance.InvalidStudentId", "Student Id must be more than 0.");

    public static readonly Error InvalidSessionOccurrenceId =
        Error.Validation("Attendance.InvalidSessionOccurrenceId", "Session Occurrence Id must be more than 0.");

    public static readonly Error InvalidStatus =
        Error.Validation("Attendance.InvalidStatus", "Status is incorrect.");
}