using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.SharedKernel.Identity;

namespace SchoolPortal.Web.Controllers;

// No IMediator here on purpose — this controller's only job is serving the
// view shell. The forms on the page talk to the minimal API endpoints
// directly via fetch, not through this controller.
[Authorize]
public sealed class AttendancesController : Controller
{
    [Authorize(Roles = Roles.Admin + "," + Roles.Teacher)]
    public IActionResult Index() => View();

    [Authorize(Roles = Roles.Admin + "," + Roles.Teacher)]
    public IActionResult Mark(int id) => View(id);

    [Authorize(Roles = Roles.Admin + "," + Roles.Teacher)]
    public IActionResult StudentHistory(int id) => View(id);

    [Authorize(Roles = Roles.Student)]
    public IActionResult MyHistory() => View();
    
    [Authorize(Roles = Roles.Admin + "," + Roles.Teacher)]
    public IActionResult Sessions() => View();
}