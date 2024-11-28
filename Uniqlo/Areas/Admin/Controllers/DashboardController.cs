using Microsoft.AspNetCore.Mvc;

namespace Uniqlo.Areas.Admin.Controllers;

[Area("Admin")]
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
