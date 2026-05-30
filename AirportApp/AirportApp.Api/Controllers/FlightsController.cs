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

        int flightId = await flightService.AddFlightAsync(
             flight.FlightNumber,
             flight.Route.Id,
             flight.Date,
             flight.Runway.Id,
             flight.Gate.Id);

         return this.Ok(flightId);
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

         await flightService.UpdateFlightAsync(
             flightId,
             flight.Date,
             flight.FlightNumber,
             flight.Runway?.Id,
             flight.Gate?.Id);

          return this.NoContent(); 
    }

    [HttpDelete("{flightId:int}")]
    public async Task<IActionResult> DeleteFlightUsingId(int flightId)
    {
        if (await flightService.GetFlightByIdAsync(flightId) == null)
        {
            return this.NotFound();
        }
         await flightService.DeleteFlightAsync(flightId);
         return this.NoContent(); 
    }
}