namespace SchoolPortal.Identities.Domain.Permissions;

/// <summary>
/// Represents a permission that can be assigned to a role.
///
/// The permission names themselves are defined in SharedKernel.Permissions.
/// This entity represents the database record for those permissions.
/// </summary>
public sealed class Permission
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}