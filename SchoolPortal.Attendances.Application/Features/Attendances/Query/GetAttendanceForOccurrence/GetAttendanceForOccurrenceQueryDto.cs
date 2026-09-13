using SchoolPortal.Attendances.Domain.Attendances;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Query;

public sealed record GetAttendanceForOccurrenceQueryDto(int AttendanceId, int StudentId, AttendanceStatus Status, string? Note);