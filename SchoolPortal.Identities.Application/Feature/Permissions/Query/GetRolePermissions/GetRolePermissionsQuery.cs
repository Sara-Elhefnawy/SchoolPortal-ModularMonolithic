using MediatR;
using SchoolPortal.Identities.Domain.Permissions;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Permissions.Query.GetRolePermissions;

public sealed record GetRolePermissionsQuery(int RoleId) : IRequest<Result<IReadOnlyList<GetRolePermissionsQueryDto>>>;