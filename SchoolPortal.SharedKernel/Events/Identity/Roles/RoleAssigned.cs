namespace SchoolPortal.SharedKernel.Domain.Events.Identity.Roles;

public sealed record RoleAssigned(int UserId, string RoleName) : IDomainEvent;