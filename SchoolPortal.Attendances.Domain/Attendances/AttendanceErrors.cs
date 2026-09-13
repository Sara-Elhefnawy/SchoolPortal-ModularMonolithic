using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Attendances.Domain.Attendances;

public static class AttendanceErrors
{
    public static readonly Error InvalidStudentId =
        Error.Validation("Attendance.InvalidStudentId", "Student id must be a positive number.");

    public static readonly Error InvalidSessionOccurrenceId =
        Error.Validation("Attendance.InvalidSessionOccurrenceId", "Session occurrence id must be a positive number.");

    public static readonly Error InvalidStatus =
        Error.Validation("Attendance.InvalidStatus", "Attendance status is not a valid value.");

    public static readonly Error InvalidNote =
        Error.Validation("Attendance.InvalidNote", "Note must be 250 characters or fewer.");

    public static readonly Error AlreadyRecorded =
        Error.Validation("Attendance.AlreadyRecorded", "attendance already recorded for the student id in this session.");

    public static readonly Error NotFound =
        Error.Validation("Attendance.NotFound", "attendance id id not found.");
}