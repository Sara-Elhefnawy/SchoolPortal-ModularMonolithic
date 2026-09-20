using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.Identities.Infrastructure.Persistence;

namespace SchoolPortal.Identities.Infrastructure.Authorization;

/// <summary>
/// Builds the authenticated ClaimsPrincipal used by ASP.NET Core.
///
/// Identity already adds standard claims such as
///
///     User ID
///     User name
///     Roles
///
/// We additionally load permissions from our database.
///
/// Example:
///
/// Teacher
///     ↓
/// RolePermissions table
///     ↓
/// Grades.View
/// Grades.Create
/// Grades.Update
///     ↓
/// permission claims
/// </summary>
public sealed class ApplicationUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor,
    IdentityDbContext dbContext)
    : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>(
        userManager,
        roleManager,
        optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        // Let ASP.NET Core Identity create its normal claims first.
        var identity = await base.GenerateClaimsAsync(user);

        // Find the roles assigned to this user.
        var roles = await userManager.GetRolesAsync(user);
        
        // Convert role names into role IDs.
        var roleIds = await dbContext.Roles
            .Where(x => x.Name != null && roles.Contains(x.Name))
            .Select(x => x.Id)
            .ToListAsync();
        
        // Load all permissions assigned to those roles.
        var permissions = await dbContext.RolePermissions
            .Where(x => roleIds.Contains(x.RoleId))
            .Select(x => x.Permission.Name)
            .Distinct()
            .ToListAsync();

        // Add each permission as a claim.
        foreach (var permission in permissions)
            identity.AddClaim(new Claim("permission", permission));

        return identity;
    }
}