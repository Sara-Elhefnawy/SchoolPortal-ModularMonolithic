namespace SchoolPortal.SharedKernel.Domain.Events.Identity.Users;

public sealed record UserDeactivated(int UserId) : IDomainEvent;