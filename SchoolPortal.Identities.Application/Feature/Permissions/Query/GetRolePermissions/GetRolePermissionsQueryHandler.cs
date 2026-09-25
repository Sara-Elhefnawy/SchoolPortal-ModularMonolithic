using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Permissions.Query.GetRolePermissions;

public sealed class GetRolePermissionsQueryHandler (
    IIdentityDirectory directory)
    : IRequestHandler<GetRolePermissionsQuery, Result<IReadOnlyList<GetRolePermissionsQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetRolePermissionsQueryDto>>> Handle(GetRolePermissionsQuery request, CancellationToken ct)
    {
        var rolePermissions = await directory.GetRolePermissionsAsync(request.RoleId, ct);

        var result = rolePermissions.Select(x =>
            new GetRolePermissionsQueryDto(x.RoleId, x.Role.Name!, x.PermissionId, x.Permission.Name)).ToList();
        
        return Result<IReadOnlyList<GetRolePermissionsQueryDto>>.Success(result);
    }
}