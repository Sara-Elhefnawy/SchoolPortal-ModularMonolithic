using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Command.UpdatePermissions;

public sealed record UpdateRolesPermissionsCommand(int RoleId, IReadOnlyList<int> PermissionIds) : IRequest<Result>;