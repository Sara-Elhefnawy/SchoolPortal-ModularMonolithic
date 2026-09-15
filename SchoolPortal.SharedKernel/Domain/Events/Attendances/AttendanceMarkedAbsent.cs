namespace SchoolPortal.SharedKernel.Domain.Events.Attendances;

public sealed record AttendanceMarkedAbsent(int AttendanceId, int StudentId, int SessionOccurrenceId) : IDomainEvent;