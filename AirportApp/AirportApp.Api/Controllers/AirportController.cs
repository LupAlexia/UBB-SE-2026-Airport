using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AirportsController : ControllerBase
{
    private readonly IAirportService airportService;

    public AirportsController(IAirportService airportService)
    {
        this.airportService = airportService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AirportDTO>>> GetAllAirports()
    {
        IEnumerable<Airport> airports = await airportService.GetAllAirportsAsync();
        return Ok(airports.Select(airport =>
            new AirportDTO(airport.Id, airport.AirportCode, airport.City, airport.Name)));
    }

    [HttpGet("{airportId}")]
    public async Task<ActionResult<AirportDTO>> GetAirportById(int airportId)
    {
        Airport? airport = await airportService.GetAirportByIdAsync(airportId);
        if (airport == null)
        {
            return NotFound();
        }

        return Ok(new AirportDTO(airport.Id, airport.AirportCode, airport.City, airport.Name));
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddAirport([FromBody] AddAirportRequest request)
    {
        try
        {
            Airport newAirport = new Airport(request.AirportCode, request.City, request.Name);
            await airportService.AddAirportAsync(newAirport);
            return Ok(newAirport.Id);
        }
        catch (ArgumentException argumentException)
        {
            return BadRequest(argumentException.Message);
        }
    }

    [HttpPut("{airportId}")]
    public async Task<IActionResult> UpdateAirport(int airportId, [FromBody] SaveAirportRequest request)
    {
        Airport airportToSave = new Airport(airportId, request.AirportCode, request.City, request.Name);
        await airportService.SaveAirportAsync(airportToSave);
        return Ok();
    }

    [HttpDelete("{airportId}")]
    public async Task<IActionResult> DeleteAirportUsingId(int airportId)
    {
        await airportService.DeleteAirportAsync(airportId);
        return Ok();
    }

    [HttpGet("{airportId}/has-flights")]
    public async Task<ActionResult<bool>> HasFlights(int airportId)
    {
        bool hasFlights = await airportService.HasFlightsAsync(airportId);
        return Ok(hasFlights);
    }

    [HttpGet("{airportId}/delete-warning")]
    public async Task<ActionResult<string>> GetDeleteWarningMessage(int airportId)
    {
        string warningMessage = await airportService.GetDeleteWarningMessageAsync(airportId);
        return Ok(new { WarningMessage = warningMessage });
    }

    public record AddAirportRequest(string AirportCode, string Name, string City);
    public record SaveAirportRequest(string AirportCode, string Name, string City);
}
