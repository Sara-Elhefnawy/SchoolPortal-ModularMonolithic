namespace SchoolPortal.SharedKernel.Domain.Events.Attendances;

public sealed record AttendanceMarkedPresent(int AttendanceId, int StudentId, int SessionOccurrenceId) : IDomainEvent;