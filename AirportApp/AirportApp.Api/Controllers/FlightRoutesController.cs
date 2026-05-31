using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Route = AirportApp.ClassLibrary.Entity.Domain.Route;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/flight-routes")]
public class FlightRoutesController(IFlightRouteService flightRouteService) : ControllerBase
{
    private const string NullRequestErrorMessage = "Request data cannot be null.";

    [HttpPost]
    public async Task<ActionResult<int>> AddFlightToRoute([FromBody] AddFlightToRouteRequest request)
    {
        if (request == null)
        {
            return this.BadRequest(NullRequestErrorMessage);
        }

        try
        {
            int routeId = await flightRouteService.AddFlightToRouteAsync(
                request.CompanyId,
                request.AirportId,
                request.RouteType,
                request.RecurrenceInterval,
                request.StartDate,
                request.EndDate,
                request.DepartureTime,
                request.ArrivalTime,
                request.Capacity,
                request.FlightNumber,
                request.RunwayId,
                request.GateId);

            return this.Ok(routeId);
        }
        catch (InvalidOperationException ex)
        {
            return this.Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpGet("flights")]
    public async Task<ActionResult<IEnumerable<Flight>>> GetAllFlights()
    {
        return this.Ok(await flightRouteService.GetAllFlightsAsync());
    }

    [HttpGet("flights/details")]
    public async Task<ActionResult<IEnumerable<Flight>>> GetAllFlightsWithDetails()
    {
        return this.Ok(await flightRouteService.GetAllFlightsWithDetailsAsync());
    }

    [HttpGet("flights/{flightId:int}")]
    public async Task<ActionResult<Flight>> GetFlightById(int flightId)
    {
        Flight? flight = await flightRouteService.GetFlightByIdAsync(flightId);

        if (flight == null)
        {
            return this.NotFound();
        }

        return this.Ok(flight);
    }

    [HttpGet("flights/by-company/{companyId:int}")]
    public async Task<ActionResult<IEnumerable<Flight>>> GetFlightsByCompanyId(int companyId)
    {
        return this.Ok(await flightRouteService.GetFlightsByCompanyIdAsync(companyId));
    }

    [HttpDelete("flights/{flightId:int}")]
    public async Task<IActionResult> DeleteFlightUsingId(int flightId)
    {
        if (await flightRouteService.GetFlightByIdAsync(flightId) == null)
        {
            return this.NotFound();
        }

        await flightRouteService.DeleteFlightUsingIdAsync(flightId);

        return this.NoContent();
    }

    [HttpGet("routes")]
    public async Task<ActionResult<IEnumerable<Route>>> GetAllRoutes()
    {
        return this.Ok(await flightRouteService.GetAllRoutesAsync());
    }

    [HttpGet("routes/{routeId:int}")]
    public async Task<ActionResult<Route>> GetRouteById(int routeId)
    {
        Route? route = await flightRouteService.GetRouteByIdAsync(routeId);

        if (route == null)
        {
            return this.NotFound();
        }

        return this.Ok(route);
    }
}