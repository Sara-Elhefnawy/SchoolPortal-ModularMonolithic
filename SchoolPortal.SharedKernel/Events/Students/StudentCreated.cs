namespace SchoolPortal.SharedKernel.Domain.Events.Students;

public sealed record StudentCreated(
    int StudentId,
    string Name,
    string Email) : IDomainEvent;