using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.SharedKernel.Identity;

namespace SchoolPortal.Web.Controllers;

// No IMediator here on purpose — this controller's only job is serving the
// view shell. The forms on the page talk to the minimal API endpoints
// directly via fetch, not through this controller.
[Authorize]
public sealed class GradesController : Controller
{
    public IActionResult Index() => View();

    [Authorize(Roles = Roles.Admin + "," + Roles.Teacher)]
    public IActionResult Gradebook() => View();

    public IActionResult Details(int id) => View();

    // Rankings: class-wide data, staff only.
    [Authorize(Roles = Roles.Admin + "," + Roles.Teacher)]
    public IActionResult Rankings() => View();

    // Student self-service page.
    [Authorize(Roles = Roles.Student)]
    public IActionResult MyGrades() => View();
    
    [Authorize(Roles = Roles.Admin + "," + Roles.Teacher)]
    public IActionResult Setup() => View();
}