using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/flights")]
public class FlightsController(IFlightService flightService) : ControllerBase
{
    private const string NullFlightDataErrorMessage = "Flight data cannot be null.";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Flight>>> GetAllFlights()
    {
        return Ok(await flightService.GetAllFlightsAsync());
    }

    [HttpGet("{flightId:int}")]
    public async Task<ActionResult<Flight>> GetFlightById(int flightId)
    {
        Flight? flight = await flightService.GetFlightByIdAsync(flightId);

        if (flight == null)
        {
            return NotFound();
        }

        return Ok(flight);
    }

    [HttpGet("by-route/{routeId:int}")]
    public async Task<ActionResult<IEnumerable<Flight>>> GetFlightsByRouteId(int routeId)
    {
        return Ok(await flightService.GetFlightsByRouteIdAsync(routeId));
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddFlight([FromBody] FlightRequestDTO flightRequestData)
    {
        if (flightRequestData == null)
        {
            return BadRequest(NullFlightDataErrorMessage);
        }

        int newFlightIdentifier = await flightService.AddFlightAsync(
            flightRequestData.FlightNumber,
            flightRequestData.RouteId,
            flightRequestData.DepartureDate,
            flightRequestData.RunwayId,
            flightRequestData.GateId);

        return Ok(newFlightIdentifier);
    }

    [HttpPut("{flightId:int}")]
    public async Task<IActionResult> UpdateFlight(int flightId, [FromBody] FlightRequestDTO flightRequestData)
    {
        if (flightRequestData == null)
        {
            return BadRequest(NullFlightDataErrorMessage);
        }

        if (await flightService.GetFlightByIdAsync(flightId) == null)
        {
            return NotFound();
        }

        await flightService.UpdateFlightAsync(
            flightId,
            flightRequestData.DepartureDate,
            flightRequestData.FlightNumber,
            flightRequestData.RunwayId,
            flightRequestData.GateId);

        return NoContent();
    }

    [HttpDelete("{flightId:int}")]
    public async Task<IActionResult> DeleteFlightUsingId(int flightId)
    {
        if (await flightService.GetFlightByIdAsync(flightId) == null)
        {
            return NotFound();
        }

        await flightService.DeleteFlightAsync(flightId);
        return NoContent();
    }
}

public record FlightRequestDTO(
    string FlightNumber,
    int RouteId,
    DateTime DepartureDate,
    int RunwayId,
    int GateId);
