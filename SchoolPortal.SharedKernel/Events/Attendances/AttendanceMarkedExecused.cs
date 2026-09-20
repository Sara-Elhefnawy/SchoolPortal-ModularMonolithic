namespace SchoolPortal.SharedKernel.Domain.Events.Attendances;

public sealed record AttendanceMarkedExcused(int AttendanceId, int StudentId, int SessionOccurrenceId) : IDomainEvent;