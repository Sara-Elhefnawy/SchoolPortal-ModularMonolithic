using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.Identities.Infrastructure.Persistence;

namespace SchoolPortal.Identities.Infrastructure.Authorization;

/// <summary>
/// Identity calls this every time it builds the cookie principal at sign-in, so it's the right place to add claims.
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
        
        // If this account is linked to a student, put the student ID into the authentication cookie.
        // The Students module does not need to know about the logged-in Identity user.
        // The ID is simply carried by the authenticated user.
        if (user.StudentId is { } studentId)
            identity.AddClaim(new Claim("student_id", studentId.ToString()));
        
        // "UserManager" (capital U) is the base class's own property. Using it instead of
        // the lowercase constructor parameter means only ONE copy of the reference exists
        var roles = await UserManager.GetRolesAsync(user);
        
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