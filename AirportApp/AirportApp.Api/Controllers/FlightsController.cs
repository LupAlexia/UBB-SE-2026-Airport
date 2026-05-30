using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/flights")]
public class FlightsController(IFlightService flightService) : ControllerBase
{
    private const string NullFlightDataErrorMessage = "Flight data cannot be null.";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Flight>>> GetAllFlights()
    {
        return this.Ok(await flightService.GetAllFlightsAsync());
    }

    [HttpGet("{flightId:int}")]
    public async Task<ActionResult<Flight>> GetFlightById(int flightId)
    {
        Flight? flight = await flightService.GetFlightByIdAsync(flightId);

        if (flight == null)
        {
            return this.NotFound();
        }

        return this.Ok(flight);
    }

    [HttpGet("by-route/{routeId:int}")]
    public async Task<ActionResult<IEnumerable<Flight>>> GetFlightsByRouteId(int routeId)
    {
        return this.Ok(await flightService.GetFlightsByRouteIdAsync(routeId));
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddFlight([FromBody] Flight flight)
    {
        if (flight == null)
        {
            return this.BadRequest(NullFlightDataErrorMessage);
        }

        try
        {
            int flightId = await flightService.AddFlightAsync(
                flight.FlightNumber,
                flight.Route.Id,
                flight.Date,
                flight.Runway.Id,
                flight.Gate.Id);

            return this.Ok(flightId);
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpPut("{flightId:int}")]
    public async Task<IActionResult> UpdateFlight(int flightId, [FromBody] Flight flight)
    {
        if (flight == null)
        {
            return this.BadRequest(NullFlightDataErrorMessage);
        }

        if (await flightService.GetFlightByIdAsync(flightId) == null)
        {
            return this.NotFound();
        }

        try
        {
            await flightService.UpdateFlightAsync(
                flightId,
                flight.Date,
                flight.FlightNumber,
                flight.Runway?.Id,
                flight.Gate?.Id);

            return this.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpDelete("{flightId:int}")]
    public async Task<IActionResult> DeleteFlightUsingId(int flightId)
    {
        if (await flightService.GetFlightByIdAsync(flightId) == null)
        {
            return this.NotFound();
        }

        try
        {
            await flightService.DeleteFlightAsync(flightId);
            return this.NoContent();
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            return this.BadRequest(ex.Message);
        }
    }
}