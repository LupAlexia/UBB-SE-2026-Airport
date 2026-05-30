using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using AirportApp.ClassLibrary.Entity.Domain;
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
    public ActionResult<List<Airport>> GetAllAirports()
    {
        return this.Ok(this.airportService.GetAllAirports());
    }

    [HttpGet("{airportId}")]
    public ActionResult<Airport> GetAirportById(int airportId)
    {
        var airport = this.airportService.GetAirportById(airportId);
        if (airport == null)
        {
            return this.NotFound();
        }

        return this.Ok(airport);
    }

    [HttpPost]
    public ActionResult<int> AddAirport([FromBody] AddAirportRequest request)
    {
        try
        {
            int newId = this.airportService.AddAirport(request.AirportCode, request.AirportName, request.City);
            return this.Ok(newId);
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpPut("{airportId}")]
    public IActionResult UpdateAirport(int airportId, [FromBody] SaveAirportRequest request)
    {
        this.airportService.SaveAirport(airportId, request.AirportCode, request.AirportName, request.City);
        return this.Ok();
    }

    [HttpDelete("{airportId}")]
    public IActionResult DeleteAirportUsingId(int airportId)
    {
        this.airportService.DeleteAirportUsingId(airportId);
        return this.Ok();
    }

    [HttpGet("{airportId}/has-flights")]
    public ActionResult<bool> HasFlights(int airportId)
    {
        return this.Ok(this.airportService.HasFlights(airportId));
    }

    [HttpGet("{airportId}/delete-warning")]
    public ActionResult<string> GetDeleteWarningMessage(int airportId)
    {
        string message = this.airportService.GetDeleteWarningMessage(airportId);
        return this.Ok(new { WarningMessage = message });
    }

    public record AddAirportRequest(string AirportCode, string AirportName, string City);

    public record UpdateAirportRequest(string? NewCity, string? NewName, string? NewCode);

    public record SaveAirportRequest(string AirportCode, string AirportName, string City);
}