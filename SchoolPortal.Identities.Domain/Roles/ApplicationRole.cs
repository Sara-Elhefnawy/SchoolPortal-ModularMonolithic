using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Permissions;

namespace SchoolPortal.Identities.Domain.Roles;

public sealed class ApplicationRole : IdentityRole<int>
{
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }
    public int? CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }

    public DateTime? RestoredAt { get; set; }
    public int? RestoredByUserId { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}