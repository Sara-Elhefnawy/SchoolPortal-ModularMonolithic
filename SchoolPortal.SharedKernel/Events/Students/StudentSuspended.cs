namespace SchoolPortal.SharedKernel.Domain.Events.Students;

public sealed record StudentSuspended(int StudentId, string Reason) : IDomainEvent;