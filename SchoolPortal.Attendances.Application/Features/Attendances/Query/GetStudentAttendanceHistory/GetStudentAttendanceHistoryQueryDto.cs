using SchoolPortal.Attendances.Domain.Attendances;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Query.GetStudentAttendanceHistory;

public sealed record GetStudentAttendanceHistoryDto(int SessionOccurrenceId, DateOnly Date, AttendanceStatus Status, string? Note);