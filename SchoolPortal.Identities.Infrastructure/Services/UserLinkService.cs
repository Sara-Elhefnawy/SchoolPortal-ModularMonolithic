using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SchoolPortal.Identities.Application.Services;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Infrastructure.Services;

internal sealed class UserLinkService(UserManager<ApplicationUser> userManager, ILogger<UserLinkService> logger)
    : IUserLinkService
{
    public async Task<Result> LinkByEmailAsync(string email, int studentId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure(UserErrors.InvalidEmail);

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        return await ApplyLinkAsync(user, studentId);
    }

    private async Task<Result> ApplyLinkAsync(ApplicationUser user, int studentId)
    {
        if (user.StudentId == studentId)
            return Result.Success();

        if (user.StudentId is not null)
            return Result.Failure(UserErrors.StudentAlreadyLinked);

        user.StudentId = studentId;

        var updated = await userManager.UpdateAsync(user);
        if (!updated.Succeeded)
            return Result.Failure(UserErrors.LinkFailed);

        await userManager.UpdateSecurityStampAsync(user);
        return Result.Success();
    }
}