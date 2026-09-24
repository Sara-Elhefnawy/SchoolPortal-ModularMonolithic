using Microsoft.AspNetCore.Mvc;

namespace SchoolPortal.Web.Controllers;

// No IMediator here on purpose — this controller's only job is serving the
// view shell. The forms on the page talk to the minimal API endpoints
// directly via fetch, not through this controller.
public sealed class AdminController : Controller
{
    public IActionResult Users() => View();
    
    public IActionResult UserDetails(int id) => View();
    public IActionResult Roles() => View();
    
    public IActionResult RoleDetails(int id) => View();

    public IActionResult Permissions() => View();
}