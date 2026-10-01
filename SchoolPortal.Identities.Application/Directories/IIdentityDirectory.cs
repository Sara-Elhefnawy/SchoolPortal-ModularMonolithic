using SchoolPortal.Identities.Domain.Permissions;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.Identities.Domain.Users;

namespace SchoolPortal.Identities.Application.Directories;

public interface IIdentityDirectory
{
    // Users
    Task<ApplicationUser?> GetUserByIdAsync(int userId, CancellationToken ct);

    Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync(CancellationToken ct);

    Task<IReadOnlyDictionary<int, IReadOnlyList<string>>> GetRoleNamesByUserAsync(CancellationToken ct);
    Task<IReadOnlyList<string>> GetRoleNamesForUserAsync(int userId, CancellationToken ct);
    
    // Roles
    Task<ApplicationRole?> GetRoleByIdAsync(int roleId, CancellationToken ct);
    Task<IReadOnlyList<ApplicationRole>> GetAllRoleAsync(CancellationToken ct);

    Task<ApplicationRole?> GetRoleByNameAsync(string roleName, CancellationToken ct);

    Task<bool> RoleExistsAsync(int roleId, CancellationToken ct);

    void AddRole(ApplicationRole role);
    
    // Permissions
    Task<IReadOnlyList<Permission>> GetAllPermissionsAsync(CancellationToken ct);

    Task<bool> PermissionExistsAsync(int permissionId, CancellationToken ct);

    // Role permissions
    Task<IReadOnlyList<RolePermission>> GetRolePermissionsAsync(int roleId, CancellationToken ct);

    Task RemoveRolePermissionsAsync(int roleId, CancellationToken ct);

    Task AddRolePermissionsAsync(IEnumerable<RolePermission> rolePermissions, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}