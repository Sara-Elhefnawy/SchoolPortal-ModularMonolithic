using Microsoft.AspNetCore.Mvc;

namespace SchoolPortal.Web.Controllers;

// No IMediator here on purpose — this controller's only job is serving the
// view shell. The forms on the page talk to the minimal API endpoints
// directly via fetch, not through this controller.
public sealed class StudentsController : Controller
{
    public IActionResult Index() => View();
}