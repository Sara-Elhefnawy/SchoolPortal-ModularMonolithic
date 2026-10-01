namespace SchoolPortal.Attendances.Application.Features.Attendances.Query.GetAttendanceForOccurrence;

public sealed record GetAttendanceForOccurrenceQueryDto(int Id, int StudentId, string StudentName, string Status, string? Note);