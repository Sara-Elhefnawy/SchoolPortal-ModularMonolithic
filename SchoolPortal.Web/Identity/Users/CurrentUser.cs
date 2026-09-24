using System.Security.Claims;
using SchoolPortal.SharedKernel.Identity.Contracts;

namespace SchoolPortal.Web.Identity.Users;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public int? UserId
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userId, out var id) ? id : null;
        }
    }
}