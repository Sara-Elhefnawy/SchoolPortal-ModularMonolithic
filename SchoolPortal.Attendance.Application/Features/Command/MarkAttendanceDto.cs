namespace SchoolPortal.Attendance.Application.Features.Command;

public record MarkAttendanceDto(int PresentCount, int AbsentCount, int LateCount, int EarlyLeave, int Excused, decimal AttendanceRate);