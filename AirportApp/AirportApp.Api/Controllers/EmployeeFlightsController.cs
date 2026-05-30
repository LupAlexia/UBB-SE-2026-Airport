using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/employee-flights")]
public class EmployeeFlightsController(IEmployeeFlightService employeeFlightService) : ControllerBase
{
    private const string NullFlightDataErrorMessage = "Flight data cannot be null.";
    private const string NullAssignmentDataErrorMessage = "Assignment data cannot be null.";

    [HttpGet("flights/{flightId:int}/employees")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesAssignedToFlight(int flightId)
    {
        var result = await employeeFlightService.GetEmployeesAssignedToFlightAsync(flightId);
        return this.Ok(result);
    }

    [HttpGet("employees/{employeeId:int}/schedule")]
    public async Task<ActionResult<IEnumerable<Flight>>> GetEmployeeSchedule(int employeeId)
    {
        var result = await employeeFlightService.GetEmployeeScheduleAsync(employeeId);
        return this.Ok(result);
    }

    [HttpGet("employees/{employeeId:int}/formatted-schedule")]
    public async Task<ActionResult<IEnumerable<EmployeeScheduleItem>>> GetFormattedEmployeeSchedule(int employeeId)
    {
        var result = await employeeFlightService.GetFormattedEmployeeScheduleAsync(employeeId);
        return this.Ok(result);
    }

    [HttpGet("employees/{employeeId:int}/available")]
    public async Task<ActionResult<bool>> IsEmployeeAvailable(int employeeId, [FromQuery] DateTime targetDate,
        [FromQuery] int targetRouteId, [FromQuery] int? excludedFlightId)
    {
        bool isAvailable = await employeeFlightService.IsEmployeeAvailableAsync(employeeId, targetDate, targetRouteId, excludedFlightId);
        return this.Ok(isAvailable);
    }

    [HttpGet("flights/{flightId:int}/crew-list")]
    public ActionResult<string> FormatCrewList(int flightId)
    {
        return this.Ok(new { result = employeeFlightService.FormatCrewList(flightId) });
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignEmployeeToFlight([FromBody] AssignEmployeeDto dto)
    {
        if (dto == null)
        {
            return this.BadRequest(NullAssignmentDataErrorMessage);
        }

        await employeeFlightService.AssignEmployeeToFlightUsingIdsAsync(dto.FlightId, dto.EmployeeId);
        return this.NoContent();
    }

    [HttpPost("flights/{flightId:int}/employees")]
    public async Task<IActionResult> AssignEmployeesToFlight(int flightId, [FromBody] List<int> employeeIds)
    {
        if (employeeIds == null)
        {
            return this.BadRequest(NullAssignmentDataErrorMessage);
        }

        await employeeFlightService.AssignEmpolyeesToFlightUsingIdsAsync(flightId, employeeIds);
        return this.NoContent();
    }

    [HttpPut("flights/{flightId:int}/employees")]
    public async Task<IActionResult> UpdateEmployeesForFlight(int flightId, [FromBody] List<int> updatedEmployeeIds)
    {
        if (updatedEmployeeIds == null)
        {
            return this.BadRequest(NullAssignmentDataErrorMessage);
        }

        await employeeFlightService.UpdateEmployeesForFlightUsingIdsAsync(flightId, updatedEmployeeIds);
        return this.NoContent();
    }

    [HttpGet("flights/{flightId:int}/available-employees")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAvailableEmployeesGroupedByRole(int flightId)
    {
        var result = await employeeFlightService.GetAvailableEmployeesGroupedByRoleByIdAsync(flightId);
        return this.Ok(result);
    }

    [HttpGet("flights/{flightId:int}/crew-selection-data")]
    public async Task<ActionResult<IEnumerable<CrewMemberSelectionData>>> GetCrewSelectionData(int flightId)
    {
        var result = await employeeFlightService.GetCrewSelectionDataByIdAsync(flightId);
        return this.Ok(result);
    }

    [HttpDelete("flights/{flightId:int}/employees/{employeeId:int}")]
    public async Task<IActionResult> RemoveEmployeeFromFlight(int flightId, int employeeId)    {
        await employeeFlightService.RemoveEmployeeFromFlightUsingIdsAsync(flightId, employeeId);
        return this.NoContent();
    }

    [HttpDelete("flights/{flightId:int}")]
    public async Task<IActionResult> RemoveAllCrewAssignmentsForFlight(int flightId)
    {
        await employeeFlightService.RemoveAllCrewAssignmentsForFlightAsync(flightId);
        return this.NoContent();
    }

    [HttpDelete("employees/{employeeId:int}")]
    public async Task<IActionResult> RemoveAllFlightsAssignmentsForEmployee(int employeeId)
    {
        await employeeFlightService.RemoveAllFlightsAssignmentsForEmployeeAsync(employeeId);
        return this.NoContent();
    }
}