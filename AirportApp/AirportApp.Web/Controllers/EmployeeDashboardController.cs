using Microsoft.AspNetCore.Mvc;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Web.Models.AirportManagement;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AirportApp.Web.Controllers;

[Authorize(Roles = "Employee")]
public class EmployeeDashboardController : Controller
{
    private readonly IEmployeeService employeeService;
    private readonly IEmployeeFlightService employeeFlightService;

    public EmployeeDashboardController(
        IEmployeeService employeeService,
        IEmployeeFlightService employeeFlightService)
    {
        this.employeeService = employeeService;
        this.employeeFlightService = employeeFlightService;
    }

    public async Task<IActionResult> Index()
    {
        int employeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var employee = await employeeService.GetEmployeeByIdAsync(employeeId);
        var schedule = await employeeFlightService.GetFormattedEmployeeScheduleAsync(employeeId);

        var model = new EmployeeDashboardViewModel
        {
            EmployeeId = employeeId,
            EmployeeName = employee?.Name ?? string.Empty,
            Role = employee?.Role.ToString() ?? string.Empty,
            Schedule = schedule,
        };

        return View(model);
    }
}