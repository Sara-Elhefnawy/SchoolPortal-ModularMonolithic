using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Identities.Domain.Permissions;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.SharedKernel.Identity;

namespace SchoolPortal.Identities.Infrastructure.Persistence.Seeders;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<IdentityDbContext>();

        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        await context.Database.MigrateAsync();

        await SeedPermissionsAsync(context);

        await SeedRolesAsync(roleManager);

        await SeedRolePermissionsAsync(context);
    }

    private static async Task SeedPermissionsAsync(IdentityDbContext context)
    {
        // Get the permissions already stored in the database.
        var existingNames = await context.Permissions.Select(x => x.Name).ToListAsync();

        // Add any permission from Permissions.All that does not already exist.
        foreach (var permissionName in Permissions.All)
        {
            if (existingNames.Contains(permissionName))
                continue;

            context.Permissions.Add(new Permission { Name = permissionName });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        ICollection<string> roles =
        [
            Roles.Admin,
            Roles.Teacher,
            Roles.Student
        ];

        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role))
                continue;

            await roleManager.CreateAsync(new ApplicationRole { Name = role });
        }
    }

    private static async Task SeedRolePermissionsAsync(IdentityDbContext context)
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

        var permissions = await context.Permissions.ToDictionaryAsync(x => x.Name);

        // We need the actual Identity role IDs because RolePermission stores RoleId.
        var roles = await context.Roles.ToDictionaryAsync(x => x.Name!);

        foreach (var (roleName, permissionNames) in initialAssignments)
        {
            var role = roles[roleName];

            foreach (var permissionName in permissionNames)
            {
                var permission = permissions[permissionName];

                var exists =
                    await context.RolePermissions.AnyAsync(x => x.RoleId == role.Id && x.PermissionId == permission.Id);

                if (exists)
                    continue;

                context.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permission.Id });
            }
        }

        await context.SaveChangesAsync();
    }
}