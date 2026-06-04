using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IAuthService authService,
    IUserService userService,
    IAdministratorService administratorService,
    IEmployeeService employeeService,
    IManagerService managerService,
    IClientService clientService) : ControllerBase
{
    /// <summary>
    /// Unified login for all roles.
    /// - role "customer": requires Email + Password (hash-verified).
    /// - roles "user" | "admin" | "employee" | "manager" | "client": requires Id (lookup only, no password).
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> LoginAsync([FromBody] UnifiedLoginRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Role))
            return BadRequest("Role is required.");

        try
        {
            return request.Role.ToLowerInvariant() switch
            {
                "customer" => await LoginCustomerAsync(request),
                "employee" => await LoginByIdAsync<Employee>(request),
                "admin" => await LoginByIdAsync<Administrator>(request),
                "user" => await LoginByIdAsync<User>(request),
                "manager" => await LoginByIdAsync<Manager>(request),
                "client" => await LoginByIdAsync<Client>(request),
                _ => BadRequest($"Unknown role '{request.Role}'. Valid roles: customer, user, admin, employee, manager, client.")
            };
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("Invalid email or password") || ex.Message.Contains("No account found"))
            {
                return Unauthorized(ex.Message);
            }
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterRequestDTO request)
    {
        try
        {
            await authService.RegisterAsync(request.Email, request.Phone, request.Username, request.Password);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task<ActionResult<LoginResponseDTO>> LoginCustomerAsync(UnifiedLoginRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email is required for customer login.");
        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Password is required for customer login.");

        Customer customer = await authService.LoginAsync(request.Email, request.Password, request.CurrentUserId);
        return Ok(new LoginResponseDTO
        {
            Id = customer.Id,
            DisplayName = customer.Username,
            Role = "customer",
            Email = customer.Email,
            Token = "mock-jwt-token"
        });
    }

    private async Task<ActionResult<LoginResponseDTO>> LoginByIdAsync<T>(UnifiedLoginRequestDTO request)
    {
        if (!request.Id.HasValue)
            return BadRequest($"Id is required for {request.Role} login.");

        int id = request.Id.Value;
        string role = request.Role.ToLowerInvariant();

        if (typeof(T) == typeof(Employee))
        {
            Employee? employee = await employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound($"Employee with ID {id} was not found.");
            return Ok(new LoginResponseDTO { Id = employee.Id, DisplayName = employee.Name, Role = role, Email = string.Empty, Token = "mock-jwt-token" });
        }

        if (typeof(T) == typeof(Administrator))
        {
            Administrator? admin = await administratorService.GetAdministratorByIdAsync(id);
            if (admin == null)
                return NotFound($"Administrator with ID {id} was not found.");
            return Ok(new LoginResponseDTO { Id = admin.Id, DisplayName = admin.FullName, Role = role, Email = admin.EmailAddress, Token = "mock-jwt-token" });
        }

        if (typeof(T) == typeof(User))
        {
            User user = await userService.GetByIdAsync(id);
            return Ok(new LoginResponseDTO { Id = user.Id, DisplayName = user.FullName, Role = role, Email = user.EmailAddress, Token = "mock-jwt-token" });
        }

        if (typeof(T) == typeof(Manager))
        {
            Manager? manager = await managerService.GetManagerByIdAsync(id);
            if (manager == null)
                return NotFound($"Manager with ID {id} was not found.");
            return Ok(new LoginResponseDTO { Id = manager.Id, DisplayName = manager.Name, Role = role, Email = manager.Email, Token = "mock-jwt-token" });
        }

        if (typeof(T) == typeof(Client))
        {
            Client? client = await clientService.GetClientByIdAsync(id);
            if (client == null)
                return NotFound($"Client with ID {id} was not found.");
            return Ok(new LoginResponseDTO { Id = client.Id, DisplayName = client.Name, Role = role, Email = string.Empty, Token = "mock-jwt-token" });
        }

        return BadRequest($"Unsupported entity type for role '{role}'.");
    }
}
