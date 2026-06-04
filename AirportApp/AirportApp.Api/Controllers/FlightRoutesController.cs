using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
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
    public async Task<ActionResult<IEnumerable<FlightDTO>>> GetAllFlights()
    {
        var flights = await flightRouteService.GetAllFlightsAsync();
        return Ok(flights.Select(MapToFlightDTO));
    }

    [HttpGet("flights/details")]
    public async Task<ActionResult<IEnumerable<FlightDTO>>> GetAllFlightsWithDetails()
    {
        var flights = await flightRouteService.GetAllFlightsWithDetailsAsync();
        return Ok(flights.Select(MapToFlightDTO));
    }

    [HttpGet("flights/{flightId:int}")]
    public async Task<ActionResult<FlightDTO>> GetFlightById(int flightId)
    {
        Flight? flight = await flightRouteService.GetFlightByIdAsync(flightId);

        if (flight == null)
        {
            return this.NotFound();
        }

        return this.Ok(MapToFlightDTO(flight));
    }

    [HttpGet("flights/by-company/{companyId:int}")]
    public async Task<ActionResult<IEnumerable<FlightDTO>>> GetFlightsByCompanyId(int companyId)
    {
        var flights = await flightRouteService.GetFlightsByCompanyIdAsync(companyId);
        return Ok(flights.Select(MapToFlightDTO));
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
    public async Task<ActionResult<IEnumerable<RouteDTO>>> GetAllRoutes()
    {
        var routes = await flightRouteService.GetAllRoutesAsync();
        return Ok(routes.Select(MapToRouteDataTransferObject));
    }

    [HttpGet("routes/{routeId:int}")]
    public async Task<ActionResult<RouteDTO>> GetRouteById(int routeId)
    {
        Route? route = await flightRouteService.GetRouteByIdAsync(routeId);

        if (route == null)
        {
            return this.NotFound();
        }

        return this.Ok(MapToRouteDataTransferObject(route));
    }

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

        RunwayDTO? runwayDto = flight.Runway is { Id: > 0 } runway ? runway.ToDto() : null;
        GateDTO? gateDto = flight.Gate is { Id: > 0 } gate ? gate.ToDto() : null;

        return new FlightDTO(
            flight.Id,
            flight.Route?.Id ?? 0,
            flight.Gate?.Id ?? 0,
            flight.Gate?.GateName ?? string.Empty,
            flight.Runway?.Id ?? 0,
            flight.Runway?.Name ?? string.Empty,
            flight.Date,
            flight.FlightNumber,
            routeDTO,
            runwayDto,
            gateDto);
    }

    private static RouteDTO MapToRouteDataTransferObject(Route route)
    {
        AirportDTO? airportDataTransferObject = route.Airport != null
            ? new AirportDTO(
                route.Airport.Id,
                route.Airport.AirportCode,
                route.Airport.City,
                route.Airport.Name)
            : null;

        CompanyDTO? companyDataTransferObject = route.Company != null
            ? new CompanyDTO(route.Company.Id, route.Company.Name)
            : null;

        return new RouteDTO(
            route.Id,
            route.RouteType,
            route.StartDate,
            route.EndDate,
            route.DepartureTime,
            route.ArrivalTime,
            route.Capacity,
            route.RecurrenceInterval,
            airportDataTransferObject,
            companyDataTransferObject);
    }
}
