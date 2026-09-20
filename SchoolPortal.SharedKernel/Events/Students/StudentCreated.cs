namespace SchoolPortal.SharedKernel.Domain.Events.Students;

public sealed record StudentCreated(int StudentId, string FullName) : IDomainEvent;