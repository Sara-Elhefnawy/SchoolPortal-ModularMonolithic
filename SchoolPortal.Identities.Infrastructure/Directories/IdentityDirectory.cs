using Microsoft.EntityFrameworkCore;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Domain.Permissions;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.Identities.Infrastructure.Persistence;

namespace SchoolPortal.Identities.Infrastructure.Directories;

internal sealed class IdentityDirectory(IdentityDbContext dbContext) : IIdentityDirectory
{
    public async Task<ApplicationUser?> GetUserByIdAsync(int userId, CancellationToken ct) =>
        await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted, ct);

    public async Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync(CancellationToken ct) =>
        await dbContext.Users.Where(x => !x.IsDeleted).OrderBy(x => x.Name).ToListAsync(ct);

    public async Task<IReadOnlyDictionary<int, IReadOnlyList<string>>> GetRoleNamesByUserAsync(CancellationToken ct)
    {
        var rows = await (from ur in dbContext.UserRoles
            join r in dbContext.Roles on ur.RoleId equals r.Id
            select new { ur.UserId, r.Name }).ToListAsync(ct);

        return rows.GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(x => x.Name!).ToList());
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesForUserAsync(int userId, CancellationToken ct)
        => await (from ur in dbContext.UserRoles
            join r in dbContext.Roles on ur.RoleId equals r.Id
            where ur.UserId == userId
            select r.Name!).ToListAsync(ct);

    public async Task<ApplicationRole?> GetRoleByIdAsync(int roleId, CancellationToken ct) =>
        await dbContext.Roles.FirstOrDefaultAsync(x => x.Id == roleId && !x.IsDeleted, ct);

    public async Task<IReadOnlyList<ApplicationRole>> GetAllRoleAsync(CancellationToken ct) =>
        await dbContext.Roles.Where(x => !x.IsDeleted).OrderBy(x => x.Name).ToListAsync(ct);

    public async Task<ApplicationRole?> GetRoleByNameAsync(string roleName, CancellationToken ct) =>
        await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == roleName && !x.IsDeleted, ct);

    public async Task<bool> RoleExistsAsync(int roleId, CancellationToken ct) =>
        await dbContext.Roles.AnyAsync(x => x.Id == roleId && !x.IsDeleted, ct);

    public void AddRole(ApplicationRole role) => dbContext.Roles.Add(role);

    public async Task<IReadOnlyList<Permission>> GetAllPermissionsAsync(CancellationToken ct) =>
        await dbContext.Permissions.OrderBy(x => x.Name).ToListAsync(ct);

    public async Task<bool> PermissionExistsAsync(int permissionId, CancellationToken ct) =>
        await dbContext.Permissions.AnyAsync(x => x.Id == permissionId, ct);

    public async Task<IReadOnlyList<RolePermission>> GetRolePermissionsAsync(int roleId, CancellationToken ct) =>
        await dbContext.RolePermissions.Include(x => x.Permission).Include(x => x.Role).Where(x => x.RoleId == roleId)
            .ToListAsync(ct);

    public async Task RemoveRolePermissionsAsync(int roleId, CancellationToken ct) =>
        await dbContext.RolePermissions.Where(x => x.RoleId == roleId).ExecuteDeleteAsync(ct);

    public async Task AddRolePermissionsAsync(IEnumerable<RolePermission> rolePermissions, CancellationToken ct) =>
        await dbContext.RolePermissions.AddRangeAsync(rolePermissions, ct);

    public async Task SaveChangesAsync(CancellationToken ct) => await dbContext.SaveChangesAsync(ct);
}