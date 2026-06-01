using System.Security.Claims;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Web.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Web.Controllers;

public class AuthController : Controller
{
    private static readonly TimeSpan CookieLifetime = TimeSpan.FromHours(8);
    private readonly IAuthService _authService;
    private readonly IAdministratorService _administratorService;
    private readonly IEmployeeService _employeeService;
    private readonly IManagerService _managerService;

    public AuthController(
        IAuthService authService,
        IAdministratorService administratorService,
        IEmployeeService employeeService,
        IManagerService managerService)
    {
        _authService = authService;
        _administratorService = administratorService;
        _employeeService = employeeService;
        _managerService = managerService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ChooseRole()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("RedirectUser", "Dashboard");
        }
        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult EnterId(string role)
    {
        if (string.IsNullOrEmpty(role))
        {
            return RedirectToAction(nameof(ChooseRole));
        }

        ViewData["Role"] = role;
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnterId(string role, string identificationId)
    {
        if (string.IsNullOrEmpty(identificationId))
        {
            ModelState.AddModelError(string.Empty, "The ID cannot be empty.");
            ViewData["Role"] = role;
            return View();
        }

        if (!int.TryParse(identificationId, out int parsedId))
        {
            ModelState.AddModelError(string.Empty, "The ID must be a valid number.");
            ViewData["Role"] = role;
            return View();
        }

        try
        {
            switch (role)
            {
                case "Admin":
                    var admin = await _administratorService.GetAdministratorByIdAsync(parsedId);
                    if (admin == null)
                    {
                        ModelState.AddModelError(string.Empty, "Admin ID not found.");
                        ViewData["Role"] = role;
                        return View();
                    }
                    await SignInAsync(admin.Id.ToString(), admin.FullName, admin.EmailAddress, new[] { "Admin" });
                    break;

                case "Employee":
                    var employee = await _employeeService.GetEmployeeByIdAsync(parsedId);
                    if (employee == null)
                    {
                        ModelState.AddModelError(string.Empty, "Employee ID not found.");
                        ViewData["Role"] = role;
                        return View();
                    }
                    await SignInAsync(employee.Id.ToString(), employee.Name, $"employee{employee.Id}@airport.com", new[] { "Employee" });
                    break;

                case "Manager":
                    var manager = await _managerService.GetManagerByIdAsync(parsedId);
                    if (manager == null)
                    {
                        ModelState.AddModelError(string.Empty, "Manager ID not found.");
                        ViewData["Role"] = role;
                        return View();
                    }
                    await SignInAsync(manager.Id.ToString(), manager.Name, manager.Email, new[] { "Manager" });
                    break;

                case "Customer":
                default:
                    Customer? customer = null;
                    try
                    {
                        customer = await _authService.GetByIdAsync(parsedId);
                    }
                    catch (KeyNotFoundException)
                    {
                        customer = null;
                    }

                    if (customer == null)
                    {
                        ModelState.AddModelError(string.Empty, "Customer ID not found.");
                        ViewData["Role"] = role;
                        return View();
                    }
                    await SignInAsync(customer.Id.ToString(), customer.Username, customer.Email, new[] { "Customer" });
                    break;
            }

            return RedirectToAction("RedirectUser", "Dashboard");
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An error occurred during identification.");
            ViewData["Role"] = role;
            return View();
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new CustomerLoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(CustomerLoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (User.Identity?.IsAuthenticated == true && User.IsInRole("Customer"))
        {
            string? sessionEmail = User.FindFirstValue(ClaimTypes.Email);
            if (!string.Equals(sessionEmail, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "The email does not match the customer ID you entered.");
                return View(model);
            }
        }

        try
        {
            Customer customer = await _authService.LoginAsync(model.Email, model.Password);
            await SignInAsync(customer.Id.ToString(), customer.Username, customer.Email, new[] { "Customer" });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("CustomerSelection", "Dashboard");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(ChooseRole));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task SignInAsync(string userId, string displayName, string email, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, displayName),
            new Claim(ClaimTypes.Email, email)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.Add(CookieLifetime)
            });
    }
}