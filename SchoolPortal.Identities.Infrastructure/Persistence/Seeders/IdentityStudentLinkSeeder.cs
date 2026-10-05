using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Lookups;
using SchoolPortal.SharedKernel.Persistence.Seeders;

namespace SchoolPortal.Identities.Infrastructure.Persistence.Seeders;

internal sealed class IdentityStudentLinkSeeder(
    UserManager<ApplicationUser> userManager,
    IStudentLookup studentLookup,
    IConfiguration configuration,
    ILogger<IdentityStudentLinkSeeder> logger) : IDataSeeder
{
    // WHY 30: must run after Identity (10) created the user and Students (20) created the student.
    public int Order => 30;

    public async Task SeedAsync(CancellationToken ct)
    {
        var email = configuration["Seed:Users:Student:Email"] ?? "student@schoolportal.test";

        var user = await userManager.FindByEmailAsync(email);
        if (user is null || user.StudentId is not null)
            return;

        var studentId = await studentLookup.FindStudentIdByEmailAsync(email, ct);
        if (studentId is null)
        {
            logger.LogWarning("No student found for {Email}; user left unlinked.", email);
            return;
        }

        user.StudentId = studentId;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException("Linking student failed: " +
                                                string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}