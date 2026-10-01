using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Permissions.Query.GetRolePermissions;

public sealed class GetRolePermissionsQueryHandler (
    IIdentityDirectory directory)
    : IRequestHandler<GetRolePermissionsQuery, Result<IReadOnlyList<string>>>
{
    public async Task<Result<IReadOnlyList<string>>> Handle(GetRolePermissionsQuery request, CancellationToken ct)
    {
        var granted = await directory.GetRolePermissionsAsync(request.RoleId, ct);
        var all = await directory.GetAllPermissionsAsync(ct);

        // WHY the join is here: RolePermission only holds ids, but the page works
        // with permission NAMES (the checkboxes' values).
        var grantedIds = granted.Select(g => g.PermissionId).ToHashSet();

        IReadOnlyList<string> names = all.Where(p => grantedIds.Contains(p.Id)).Select(p => p.Name).ToList();
        
        return Result<IReadOnlyList<string>>.Success(names);
    }
}