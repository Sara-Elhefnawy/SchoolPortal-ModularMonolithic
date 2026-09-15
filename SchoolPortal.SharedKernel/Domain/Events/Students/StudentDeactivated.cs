namespace SchoolPortal.SharedKernel.Domain.Events.Students;

public sealed record StudentDeactivated(int StudentId) : IDomainEvent;