using Microsoft.AspNetCore.Mvc;

namespace SchoolPortal.Web.Controllers;

// No IMediator here on purpose — this controller's only job is serving the
// view shell. The forms on the page talk to the minimal API endpoints
// directly via fetch, not through this controller.
public sealed class AccountController : Controller
{
    public IActionResult Login() => View();

    public IActionResult Signup() => View();
    public IActionResult ForgotPassword() => View();
}