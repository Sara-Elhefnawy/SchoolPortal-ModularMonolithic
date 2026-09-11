using SchoolPortal.Attendance.Domain.Attendances;

namespace SchoolPortal.Attendance.Application.Features.Query.GetAttendanceForSession;

public record GetAttendanceForSessionDto(int StudentId, AttendanceStatus Status, string? Note);