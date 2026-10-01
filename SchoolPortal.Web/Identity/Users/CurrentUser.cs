using System.Security.Claims;
using SchoolPortal.SharedKernel.Identity.Contracts;

namespace SchoolPortal.Web.Identity.Users;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    // Read once per access instead of repeating the null-conditional chain in every property.
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public int? UserId => int.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public int? StudentId =>
        int.TryParse(User?.FindFirstValue("student_id"), out var studentId) ? studentId : null;
    
    public string? Name => User?.Identity?.Name;

    // Empty lists rather than null, so callers never need a null check.
    public IReadOnlyList<string> Roles => User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? [];

    public IReadOnlyList<string> Permissions => User?.FindAll("permission").Select(c => c.Value).ToList() ?? [];
}