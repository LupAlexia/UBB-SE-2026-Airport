using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    public IActionResult RedirectUser()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction(nameof(AdminDashboard));
        }
        if (User.IsInRole("Manager"))
        {
            return RedirectToAction(nameof(ManagerDashboard));
        }
        if (User.IsInRole("Employee"))
        {
            return RedirectToAction(nameof(EmployeeDashboard));
        }

        return RedirectToAction(nameof(CustomerSelection));
    }

    [Authorize(Roles = "Customer")]
    public IActionResult SupportDashboard()
    {
        return View();
    }

    [Authorize(Roles = "Customer")]
    public IActionResult CustomerSelection()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public IActionResult AdminDashboard()
    {
        return View();
    }

    [Authorize(Roles = "Manager")]
    public IActionResult ManagerDashboard()
    {
        return RedirectToAction("Index", "CompanyDashboard");
    }

    [Authorize(Roles = "Employee")]
    public IActionResult EmployeeDashboard()
    {
        return RedirectToAction("Index", "EmployeeDashboard");
    }
}
