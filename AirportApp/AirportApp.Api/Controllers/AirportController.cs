using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using AirportApp.ClassLibrary.Entity.Domain;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Dto;

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
    public async Task<ActionResult<List<Airport>>> GetAllAirports()
    {
        // return this.Ok(this.airportService.GetAllAirports());
        var airports = await this.airportService.GetAllAirportsAsync();
        return this.Ok(airports);
    }

    [HttpGet("{airportId}")]
    public async Task<ActionResult<Airport>> GetAirportById(int airportId)
    {
        var airport = await this.airportService.GetAirportByIdAsync(airportId);
        if (airport == null)
        {
            return this.NotFound();
        }

        return this.Ok(airport);
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddAirport([FromBody] AddAirportRequest request)
    {
        try
        {
            var newAirport = new AirportApp.ClassLibrary.Entity.Domain.Airport(request.AirportCode, request.City, request.AirportName);

            await this.airportService.AddAirportAsync(newAirport);
            return this.Ok(newAirport.Id);
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpPut("{airportId}")]
    public async Task<IActionResult> UpdateAirport(int airportId, [FromBody] SaveAirportRequest request)
    {
        // this.airportService.SaveAirport(airportId, request.AirportCode, request.AirportName, request.City);
        var airportToSave = new Airport(airportId, request.AirportCode, request.City, request.AirportName);

        await this.airportService.SaveAirportAsync(airportToSave);
        return this.Ok();
    }

    [HttpDelete("{airportId}")]
    public async Task<IActionResult> DeleteAirportUsingId(int airportId)
    {
        await this.airportService.DeleteAirportAsync(airportId);
        return this.Ok();
    }

    [HttpGet("{airportId}/has-flights")]
    public async Task<ActionResult<bool>> HasFlights(int airportId)
    {
        // return this.Ok(this.airportService.HasFlights(airportId));
        bool hasFlights = await this.airportService.HasFlightsAsync(airportId);
        return this.Ok(hasFlights);
    }

    [HttpGet("{airportId}/delete-warning")]
    public async Task<ActionResult<string>> GetDeleteWarningMessage(int airportId)
    {
        string message = await this.airportService.GetDeleteWarningMessageAsync(airportId);
        return this.Ok(new { WarningMessage = message });
    }

    public record AddAirportRequest(string AirportCode, string AirportName, string City);

    public record UpdateAirportRequest(string? NewCity, string? NewName, string? NewCode);

    public record SaveAirportRequest(string AirportCode, string AirportName, string City);
}