namespace SchoolPortal.SharedKernel.Domain.Events.Attendances;

public sealed record AttendanceMarkedEarlyLeave(int AttendanceId, int StudentId, int SessionOccurrenceId) : IDomainEvent;