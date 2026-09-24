using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolPortal.Web.Controllers;

[Authorize]
public sealed class DashboardController : Controller
{
    public IActionResult Index() => View();
}