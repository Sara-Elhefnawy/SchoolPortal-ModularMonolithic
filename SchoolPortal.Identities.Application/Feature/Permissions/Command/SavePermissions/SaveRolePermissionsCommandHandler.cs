using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Domain.Permissions;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Permissions.Command.SavePermissions;

public sealed class SaveRolePermissionsCommandHandler(
    IIdentityDirectory directory)
    : IRequestHandler<SaveRolePermissionsCommand, Result>
{
    public async Task<Result> Handle(SaveRolePermissionsCommand request, CancellationToken ct)
    {
        if (!await directory.RoleExistsAsync(request.RoleId, ct))
            return Result.Failure(RoleErrors.NotFound);

        var all = await directory.GetAllPermissionsAsync(ct);

        // WHY filter by known permissions: the body comes from the browser, so a
        // made-up name must be ignored, not turned into a row.
        var wanted = all.Where(p => request.Permissions.Contains(p.Name)).ToList();

        // WHY remove-then-add: the page sends the full list of ticked boxes, so
        // replacing is simpler than computing a diff.
        await directory.RemoveRolePermissionsAsync(request.RoleId, ct);

        // ADJUST: use RolePermission's factory/constructor if it has one.
        await directory.AddRolePermissionsAsync(
            wanted.Select(p => new RolePermission { RoleId = request.RoleId, PermissionId = p.Id }), ct);

        await directory.SaveChangesAsync(ct);
        return Result.Success();
    }
}