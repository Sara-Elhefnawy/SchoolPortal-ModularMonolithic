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
    IHostEnvironment environment,
    ILogger<IdentitySeeder> logger) : IDataSeeder
{
    public int Order => 10;

    public async Task SeedAsync(CancellationToken ct)
    {
        await context.Database.MigrateAsync(ct);

        await SeedPermissionsAsync(ct);

        await SeedRolesAsync(ct);

        await SeedRolePermissionsAsync(ct);

        // WHY Development only: these accounts have a known password. They must
        // never be created on a deployed database.
        if (environment.IsDevelopment())
            await SeedDevUsersAsync();
    }

    private async Task SeedPermissionsAsync(CancellationToken ct)
    {
        // One query for everything that already exists.
        var existingNames = (await context.Permissions.Select(x => x.Name).ToListAsync(ct)).ToHashSet();

        foreach (var permissionName in Permissions.All)
        {
            // HashSet.Add returns false when the name is already there, so this is
            // both the "already in the database" check and protection against
            // duplicates inside Permissions.All itself.
            if (!existingNames.Add(permissionName))
                continue;

            context.Permissions.Add(new Permission { Name = permissionName });
        }

        await context.SaveChangesAsync(ct);
    }

    private async Task SeedRolesAsync(CancellationToken ct)
    {
        // WHY one query instead of RoleExistsAsync per role: that was one round-trip
        // per role on every startup, even when nothing needed creating.
        var existingRoles = (await context.Roles.Select(x => x.Name!).ToListAsync(ct)).ToHashSet();

        ICollection<string> roles =
        [
            Roles.Admin,
            Roles.Teacher,
            Roles.Student
        ];

        foreach (var role in roles)
        {
            if (existingRoles.Contains(role))
                continue;

            // WHY still RoleManager for creation: it sets NormalizedName and the
            // concurrency stamp correctly, which a raw DbContext insert would skip.
            var result = await roleManager.CreateAsync(new ApplicationRole { Name = role });

            // The original ignored failures; a role that silently failed to create
            // would surface later as a confusing KeyNotFoundException.
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Seeding role '{role}' failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
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

        // We need the actual Identity role IDs because RolePermission stores RoleId.
        var roles = await context.Roles.ToDictionaryAsync(x => x.Name!, ct);

        // THE FIX for the query storm: load every existing (RoleId, PermissionId)
        // pair in ONE query, instead of one SELECT EXISTS per pair (~28 per startup).
        var existingPairs =
            (await context.RolePermissions.Select(x => new { x.RoleId, x.PermissionId }).ToListAsync(ct))
            .Select(x => (x.RoleId, x.PermissionId)).ToHashSet();

        foreach (var (roleName, permissionNames) in initialAssignments)
        {
            var role = roles[roleName];

            foreach (var permissionName in permissionNames)
            {
                var permission = permissions[permissionName];

                // Add() returns false if the pair is already known, so this both
                // checks the database state and stops duplicates within this run.
                if (!existingPairs.Add((role.Id, permission.Id)))
                    continue;

                context.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permission.Id });
            }
        }

        await context.SaveChangesAsync(ct);
    }

    private async Task SeedDevUsersAsync()
    {
        // WHY configuration and not a literal: your repo is going on a CV, so a
        // password in source is public. Set it once with:
        //   dotnet user-secrets set "Seed:DefaultPassword" "<a password that meets your Identity rules>"
        var password = configuration["Seed:DefaultPassword"];

        if (string.IsNullOrWhiteSpace(password))
        {
            // WHY warn instead of throw: someone cloning the repo (a recruiter, say)
            // should still be able to start the app. They just get no dev accounts.
            logger.LogWarning("Seed:DefaultPassword is not set, so development users were not created.");
            return;
        }

        var users = new (string Name, string Email, string Role)[]
        {
            ("Dev Admin", "admin@schoolportal.test", Roles.Admin),
            ("Dev Teacher", "teacher@schoolportal.test", Roles.Teacher),
            ("Dev Student", "student@schoolportal.test", Roles.Student)
        };

        foreach (var u in users)
        {
            if (await userManager.FindByEmailAsync(u.Email) is not null)
                continue;

            var user = new ApplicationUser
            {
                Name = u.Name,
                UserName = u.Email,
                Email = u.Email,
                // WHY confirmed: your login flow supports email confirmation, and the
                // dev email sender only logs to the console, so nobody could confirm these.
                EmailConfirmed = true,
                IsActive = true
            };

            var created = await userManager.CreateAsync(user, password);

            if (!created.Succeeded)
                throw new InvalidOperationException(
                    $"Seeding user '{u.Email}' failed: {string.Join(", ", created.Errors.Select(e => e.Description))}");

            var assigned = await userManager.AddToRoleAsync(user, u.Role);

            if (!assigned.Succeeded)
                throw new InvalidOperationException(
                    $"Assigning role '{u.Role}' to '{u.Email}' failed: {string.Join(", ", assigned.Errors.Select(e => e.Description))}");
        }
    }
}