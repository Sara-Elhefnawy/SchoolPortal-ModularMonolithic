using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Permissions.Command.SavePermissions;

public sealed record SaveRolePermissionsCommand(int RoleId, IReadOnlyCollection<string> Permissions)
    : IRequest<Result>;