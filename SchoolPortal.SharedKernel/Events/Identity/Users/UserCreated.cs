namespace SchoolPortal.SharedKernel.Domain.Events.Identity.Users;

public sealed record UserCreated(int UserId, string RoleName) : IDomainEvent;