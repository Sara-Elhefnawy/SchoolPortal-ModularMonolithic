using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolPortal.Identities.Domain.Permissions;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Identity;
using SchoolPortal.SharedKernel.Persistence.Seeders;

namespace SchoolPortal.Identities.Infrastructure.Persistence.Seeders;

// WHY internal + constructor injection: it's resolved by DI like the other seeders,
// so Web no longer needs to reference this class (or this assembly's internals).
internal sealed class IdentitySeeder(
    IdentityDbContext context,
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    ILogger<IdentitySeeder> logger) : IDataSeeder
{
    public int Order => 10;

    public async Task SeedAsync(CancellationToken ct)
    {
        await context.Database.MigrateAsync(ct);

        await SeedPermissionsAsync(ct);

        await SeedRolesAsync(ct);

        await SeedRolePermissionsAsync(ct);

        await SeedDevUsersAsync(ct);
    }

    private async Task SeedPermissionsAsync(CancellationToken ct)
    {
        var existingNames = (await context.Permissions.Select(x => x.Name).ToListAsync(ct)).ToHashSet();

        foreach (var permissionName in Permissions.All)
        {
            if (!existingNames.Add(permissionName))
                continue;

            context.Permissions.Add(new Permission { Name = permissionName });
        }

        await context.SaveChangesAsync(ct);
    }

    private async Task SeedRolesAsync(CancellationToken ct)
    {
        var existingRoles = (await context.Roles.Select(x => x.Name!).ToListAsync(ct)).ToHashSet();

        var roles = new[] { Roles.Admin, Roles.Teacher, Roles.Student };

        foreach (var role in roles)
        {
            if (existingRoles.Contains(role))
                continue;

            var result = await roleManager.CreateAsync(new ApplicationRole { Name = role });

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Seeding role '{role}' failed: " +
                                                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private async Task SeedRolePermissionsAsync(CancellationToken ct)
    {
        var initialAssignments = new Dictionary<string, string[]>
        {
            [Roles.Admin] =
            [
                Permissions.Users.View,
                Permissions.Users.Create,
                Permissions.Users.Update,
                Permissions.Users.Delete,

                Permissions.Students.View,
                Permissions.Students.Create,
                Permissions.Students.Update,
                Permissions.Students.Delete,

                Permissions.Grades.View,
                Permissions.Grades.Create,
                Permissions.Grades.Update,
                Permissions.Grades.Delete,

                Permissions.Attendance.View,
                Permissions.Attendance.Mark,
                Permissions.Attendance.Update
            ],
            [Roles.Teacher] =
            [
                Permissions.Students.View,

                Permissions.Grades.View,
                Permissions.Grades.Create,
                Permissions.Grades.Update,

                Permissions.Attendance.View,
                Permissions.Attendance.Mark,
                Permissions.Attendance.Update
            ],
            [Roles.Student] =
            [
                Permissions.Students.View,
                Permissions.Grades.View,
                Permissions.Attendance.View
            ]
        };

        var permissions = await context.Permissions.ToDictionaryAsync(x => x.Name, ct);

        var roles = await context.Roles.ToDictionaryAsync(x => x.Name!, ct);

        var existingPairs =
            (await context.RolePermissions.Select(x => new { x.RoleId, x.PermissionId }).ToListAsync(ct))
            .Select(x => (x.RoleId, x.PermissionId)).ToHashSet();

        foreach (var (roleName, permissionNames) in initialAssignments)
        {
            var role = roles[roleName];

            foreach (var permissionName in permissionNames)
            {
                var permission = permissions[permissionName];

                if (!existingPairs.Add((role.Id, permission.Id)))
                    continue;

                context.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permission.Id });
            }
        }

        await context.SaveChangesAsync(ct);
    }

    private async Task SeedDevUsersAsync(CancellationToken ct)
    {
        var password = configuration["Seed:Users:Password"];

        if (string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Seed:Users:Password is not configured. " + "Development users were not created.");

            return;
        }

        var users = new[]
        {
            new SeedUser(configuration["Seed:Users:Admin:Name"] ?? "Dev Admin",
                configuration["Seed:Users:Admin:Email"] ?? "admin@schoolportal.test", Roles.Admin),
            new SeedUser(configuration["Seed:Users:Teacher:Name"] ?? "Dev Teacher",
                configuration["Seed:Users:Teacher:Email"] ?? "teacher@schoolportal.test", Roles.Teacher),
            new SeedUser(configuration["Seed:Users:Student:Name"] ?? "Sara Ahmed",
                configuration["Seed:Users:Student:Email"] ?? "student@schoolportal.test", Roles.Student)
        };

        foreach (var seedUser in users)
        {
            await EnsureUserAsync(seedUser, password, ct);
        }
    }

    private async Task EnsureUserAsync(SeedUser seedUser, string password, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(seedUser.Email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                Name = seedUser.Name,
                UserName = seedUser.Email,
                Email = seedUser.Email,
                EmailConfirmed = true,
                IsActive = true
            };

            var created = await userManager.CreateAsync(user, password);

            if (!created.Succeeded)
            {
                throw new InvalidOperationException($"Seeding user '{seedUser.Email}' failed: " +
                                                    string.Join(", ", created.Errors.Select(e => e.Description)));
            }

            logger.LogInformation("Created development user {Email}.", seedUser.Email);
        }

        var roles = await userManager.GetRolesAsync(user);

        if (roles.Contains(seedUser.Role))
            return;

        var assigned = await userManager.AddToRoleAsync(user, seedUser.Role);

        if (!assigned.Succeeded)
        {
            throw new InvalidOperationException($"Assigning role '{seedUser.Role}' to " +
                                                $"'{seedUser.Email}' failed: " + string.Join(", ",
                                                    assigned.Errors.Select(e => e.Description)));
        }

        logger.LogInformation("Assigned role {Role} to {Email}.", seedUser.Role, seedUser.Email);
    }

    private sealed record SeedUser(string Name, string Email, string Role);
}