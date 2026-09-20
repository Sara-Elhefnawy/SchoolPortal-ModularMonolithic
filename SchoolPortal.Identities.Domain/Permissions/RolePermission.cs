using SchoolPortal.Identities.Domain.Roles;

namespace SchoolPortal.Identities.Domain.Permissions;

/// <summary>
/// Join entity between a role and a permission.
///
/// This is what will eventually allow Admin Dashboard to enable/disable permissions for roles.
/// </summary>
public sealed class RolePermission
{
    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public ApplicationRole Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}