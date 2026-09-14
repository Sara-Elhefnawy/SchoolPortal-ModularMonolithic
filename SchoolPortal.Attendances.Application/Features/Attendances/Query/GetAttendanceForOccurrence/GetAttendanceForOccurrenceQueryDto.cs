using SchoolPortal.Attendances.Domain.Attendances;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Query.GetAttendanceForOccurrence;

public sealed record GetAttendanceForOccurrenceQueryDto(int AttendanceId, int StudentId, AttendanceStatus Status, string? Note);