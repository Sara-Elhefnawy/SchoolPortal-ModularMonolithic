namespace SchoolPortal.Attendance.Application.Features.Query.GetAttendanceSummary;

public record GetAttendanceSummaryDto(int PresentCount, int AbsentCount, int LateCount, decimal AttendanceRate);