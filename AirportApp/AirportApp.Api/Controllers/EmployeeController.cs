using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    private const string NullEmployeeDataErrorMessage = "Employee data cannot be null.";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
    {
        return this.Ok(await employeeService.GetAllEmployeesAsync());
    }

    [HttpGet("{employeeId:int}")]
    public async Task<ActionResult<Employee>> GetEmployeeById(int employeeId)
    {
        Employee? employee = await employeeService.GetEmployeeByIdAsync(employeeId);

        if (employee == null)
        {
            return this.NotFound();
        }

        return this.Ok(employee);
    }

    [HttpGet("pilots")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetPilots()
    {
        return this.Ok(await employeeService.GetPilotsAsync());
    }

    [HttpGet("flight-attendants")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetFlightAttendants()
    {
        return this.Ok(await employeeService.GetFlightAttendantsAsync());
    }

    [HttpGet("co-pilots")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetCoPilots()
    {
        return this.Ok(await employeeService.GetCoPilotsAsync());
    }

    [HttpGet("flight-dispatchers")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetFlightDispatchers()
    {
        return this.Ok(await employeeService.GetFlightDispatchersAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> AddEmployee([FromBody] Employee employee)
    {
        if (employee == null)
        {
            return this.BadRequest(NullEmployeeDataErrorMessage);
        }

        int employeeId = await employeeService.AddEmployeeAsync(employee.Name, employee.Role, employee.Birthday, employee.Salary, employee.HiringDate);

        return this.Ok(employeeId);
    }

    [HttpPut("{employeeId:int}")]
    public async Task<IActionResult> UpdateEmployee(int employeeId, [FromBody] Employee employee)
    {
        if (employee == null)
        {
            return this.BadRequest(NullEmployeeDataErrorMessage);
        }

        if (await employeeService.GetEmployeeByIdAsync(employeeId) == null)
        {
            return this.NotFound();
        }

        employee.Id = employeeId;
        await employeeService.UpdateEmployeeAsync(employee.Id, employee.Name, employee.Role, employee.Salary, employee.Birthday, employee.HiringDate);

        return this.NoContent();
    }

    [HttpDelete("{employeeId:int}")]
    public async Task<IActionResult> DeleteEmployeeUsingId(int employeeId)
    {
        if (await employeeService.GetEmployeeByIdAsync(employeeId) == null)
        {
            return this.NotFound();
        }

        await employeeService.DeleteEmployeeUsingIdAsync(employeeId);

        return this.NoContent();
    }

    [HttpDelete("{employeeId:int}/with-assignments")]
    public async Task<IActionResult> DeleteWithAssignments(int employeeId)
    {
        if (await employeeService.GetEmployeeByIdAsync(employeeId) == null)
        {
            return this.NotFound();
        }

        await employeeService.DeleteWithAssignmentsAsync(employeeId);

        return this.NoContent();
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] SaveEmployeeDto dto)
    {
        if (dto?.Employee == null)
        {
            return this.BadRequest(NullEmployeeDataErrorMessage);
        }

        await employeeService.SaveEmployeeAsync(dto.Employee, dto.Birthday, dto.HiringDate, dto.SalaryText);

        return this.NoContent();
    }

    [HttpGet("parse-role")]
    public ActionResult<EmployeeRoleEnum> ParseRole([FromQuery] string roleText)
    {
        return this.Ok(employeeService.ParseRole(roleText));
    }

    [HttpGet("login")]
    public async Task<ActionResult<int>> Login([FromQuery] string employeeIdText)
    {
        int employeeId = await employeeService.LoginAsync(employeeIdText);

        return this.Ok(employeeId);
    }
}