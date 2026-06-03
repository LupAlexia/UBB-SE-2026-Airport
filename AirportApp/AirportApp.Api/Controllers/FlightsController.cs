using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/flights")]
public class FlightsController(IFlightService flightService, IFlightSearchService flightSearchService) : ControllerBase
{
    private const string NullFlightDataErrorMessage = "Flight data cannot be null.";

    // ── CRUD (from 921) ──────────────────────────────────────────────────────

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Flight>>> GetAllFlights()
    {
        return Ok(await flightService.GetAllFlightsAsync());
    }

    [HttpGet("{flightId:int}")]
    public async Task<ActionResult<FlightDTO>> GetFlightById(int flightId)
    {
        Flight? flight = await flightSearchService.GetFlightByIdAsync(flightId);
        if (flight == null)
        {
            return NotFound();
        }

        return Ok(MapToFlightDTO(flight));
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

        int newFlightId = await flightService.AddFlightAsync(
            flightRequestData.FlightNumber,
            flightRequestData.RouteId,
            flightRequestData.DepartureDate,
            flightRequestData.RunwayId,
            flightRequestData.GateId);

        return Ok(newFlightId);
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

    // ── Search / booking (from 924) ──────────────────────────────────────────

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<FlightDTO>>> SearchFlights(
        [FromQuery] string location,
        [FromQuery] string routeType,
        [FromQuery] DateTime? date)
    {
        IEnumerable<Flight> flights = await flightSearchService.GetFlightsByRouteAsync(location, routeType, date);
        return Ok(flights.Select(MapToFlightDTO));
    }

    [HttpGet("{flightId:int}/occupied-seat-count")]
    public async Task<ActionResult<int>> GetOccupiedSeatCount(int flightId)
    {
        int count = await flightSearchService.GetOccupiedSeatCountAsync(flightId);
        return Ok(count);
    }

    // ── Mapping ──────────────────────────────────────────────────────────────

    private static FlightDTO MapToFlightDTO(Flight flight)
    {
        RouteDTO? routeDTO = null;

        if (flight.Route != null)
        {
            AirportDTO? airportDTO = flight.Route.Airport != null
                ? new AirportDTO(
                    flight.Route.Airport.Id,
                    flight.Route.Airport.AirportCode,
                    flight.Route.Airport.City,
                    flight.Route.Airport.Name)
                : null;

            CompanyDTO? companyDTO = flight.Route.Company != null
                ? new CompanyDTO(flight.Route.Company.Id, flight.Route.Company.Name)
                : null;

            routeDTO = new RouteDTO(
                flight.Route.Id,
                flight.Route.RouteType,
                flight.Route.StartDate,
                flight.Route.EndDate,
                flight.Route.DepartureTime,
                flight.Route.ArrivalTime,
                flight.Route.Capacity,
                flight.Route.RecurrenceInterval,
                airportDTO,
                companyDTO);
        }

        return new FlightDTO(
            flight.Id,
            flight.Route?.Id ?? 0,
            flight.Gate?.Id ?? 0,
            flight.Gate?.GateName ?? string.Empty,
            flight.Runway?.Id ?? 0,
            flight.Runway?.Name ?? string.Empty,
            flight.Date,
            flight.FlightNumber,
            routeDTO);
    }
}

public record FlightRequestDTO(
    string FlightNumber,
    int RouteId,
    DateTime DepartureDate,
    int RunwayId,
    int GateId);
