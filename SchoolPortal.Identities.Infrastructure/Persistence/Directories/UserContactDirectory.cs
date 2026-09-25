using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Identity;
using SchoolPortal.SharedKernel.Identity.Contracts;

namespace SchoolPortal.Identities.Infrastructure.Persistence.Directories;

public class UserContactDirectory(UserManager<ApplicationUser> userManager) : IUserContactDirectory
{
    public async Task<string?> GetEmailAsync(int userId, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        return user?.Email;
    }
}