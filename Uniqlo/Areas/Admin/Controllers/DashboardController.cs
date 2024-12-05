using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uniqlo.Views.Account.Enums;

namespace Uniqlo.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = nameof(Roles.Admin))]
public class DashboardController : Controller
{
    public IActionResult Admin()
    {
        return View();
    }
    public IActionResult Index()
    {
        return View();
    }
}
