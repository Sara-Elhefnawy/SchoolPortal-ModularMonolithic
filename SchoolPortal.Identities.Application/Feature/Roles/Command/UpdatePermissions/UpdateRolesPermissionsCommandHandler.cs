using MediatR;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Domain.Permissions;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Command.UpdatePermissions;

public sealed class UpdateRolesPermissionsCommandHandler (
    IIdentityDirectory directory)
    : IRequestHandler<UpdateRolesPermissionsCommand, Result>
{
    public async Task<Result> Handle(UpdateRolesPermissionsCommand request, CancellationToken ct)
    {
        var roleExists = await directory.RoleExistsAsync(request.RoleId, ct);
        if (!roleExists)
            return Result.Failure(RoleErrors.NotFound);

        // Every requested permission must exist in the Permissions table.
        // Stop immediately if an invalid permission ID is provided.
        foreach (var permissionId in request.PermissionIds.Distinct())
        {
            var permissionExists = await directory.PermissionExistsAsync(permissionId, ct);
            if (!permissionExists)
                return Result.Failure(PermissionErrors.NotFound);
        }

        // Replace the role's current permissions with the new selection.
        await directory.RemoveRolePermissionsAsync(request.RoleId, ct);

        var rolePermissions = request.PermissionIds.Distinct().Select(permissionId =>
            new RolePermission { RoleId = request.RoleId, PermissionId = permissionId }).ToList();

        await directory.AddRolePermissionsAsync(rolePermissions, ct);

        await directory.SaveChangesAsync(ct);

        return Result.Success();
    }
}