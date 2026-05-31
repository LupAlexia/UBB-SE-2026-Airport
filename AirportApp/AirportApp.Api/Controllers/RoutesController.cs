using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Route = AirportApp.ClassLibrary.Entity.Domain.Route;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/routes")]
public class RoutesController(IRouteService routeService) : ControllerBase
{
    private const string NullRouteDataErrorMessage = "Route data cannot be null.";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RouteDTO>>> GetAll()
    {
        IEnumerable<Route> routes = await routeService.GetAllRoutesAsync();
        return Ok(routes.Select(MapToRouteDataTransferObject));
    }

    [HttpGet("{routeId:int}")]
    public async Task<ActionResult<RouteDTO>> GetById(int routeId)
    {
        Route? route = await routeService.GetRouteByIdAsync(routeId);

        if (route == null)
        {
            return NotFound();
        }

        return Ok(MapToRouteDataTransferObject(route));
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddWithInitialFlight([FromBody] AddRouteWithFlightRequest request)
    {
        if (request == null)
        {
            return BadRequest(NullRouteDataErrorMessage);
        }

        try
        {
            int routeIdentifier = await routeService.AddWithInitialFlightAsync(
                request.CompanyId,
                request.AirportId,
                request.RouteType,
                request.RecurrenceInterval,
                request.StartDate.ToDateTime(TimeOnly.MinValue),
                request.EndDate.ToDateTime(TimeOnly.MinValue),
                request.DepartureTime,
                request.ArrivalTime,
                request.Capacity,
                request.FlightNumber,
                request.RunwayId,
                request.GateId);

            return Ok(routeIdentifier);
        }
        catch (InvalidOperationException conflictException)
        {
            return Conflict(conflictException.Message);
        }
    }

    [HttpGet("normalize-type")]
    public ActionResult<string> NormalizeFlightType([FromQuery] string? routeType)
    {
        return Ok(new { value = routeService.NormalizeFlightType(routeType) });
    }

    [HttpPost("relevant-time")]
    public ActionResult<string> GetRelevantTime([FromBody] RouteTimeRequest request)
    {
        Route route = new()
        {
            RouteType = request.RouteType ?? string.Empty,
            DepartureTime = request.DepartureTime,
            ArrivalTime = request.ArrivalTime
        };

        return Ok(new { value = routeService.GetRelevantTime(route) });
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

public sealed record AddRouteWithFlightRequest(
    int CompanyId,
    int AirportId,
    string RouteType,
    int RecurrenceInterval,
    DateOnly StartDate,
    DateOnly EndDate,
    TimeOnly DepartureTime,
    TimeOnly ArrivalTime,
    int Capacity,
    string FlightNumber,
    int RunwayId,
    int GateId);

public sealed record RouteTimeRequest(string? RouteType, TimeOnly DepartureTime, TimeOnly ArrivalTime);
