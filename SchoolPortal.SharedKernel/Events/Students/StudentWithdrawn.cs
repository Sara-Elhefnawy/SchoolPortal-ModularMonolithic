namespace SchoolPortal.SharedKernel.Domain.Events.Students;

public sealed record StudentWithdrawn(int StudentId, DateOnly EffectiveDate) : IDomainEvent;