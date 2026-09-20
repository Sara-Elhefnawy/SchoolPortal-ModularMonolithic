namespace SchoolPortal.SharedKernel.Domain.Events.Attendances.SessionOccurrences;

public sealed record SessionOccurrenceCancelled(int Id, int SessionId, DateOnly Date) : IDomainEvent;