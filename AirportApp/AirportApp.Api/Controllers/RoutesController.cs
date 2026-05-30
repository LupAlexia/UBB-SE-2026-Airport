using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Route = AirportApp.ClassLibrary.Entity.Domain.Route;

namespace AirportAPI.Controllers.A5_Controllers;

[ApiController]
[Route("api/routes")]
public class RoutesController(IRouteService routeService) : ControllerBase
{
    private const string NullRouteDataErrorMessage = "Route data cannot be null.";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Route>>> GetAll()
    {
        return this.Ok(await routeService.GetAllRoutesAsync());
    }

    [HttpGet("{routeId:int}")]
    public async Task<ActionResult<Route>> GetById(int routeId)
    {
        Route? route = await routeService.GetRouteByIdAsync(routeId);

        if (route == null)
        {
            return this.NotFound();
        }

        return this.Ok(route);
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddWithInitialFlight([FromBody] AddRouteWithFlightRequest request)
    {
        if (request == null)
        {
            return this.BadRequest(NullRouteDataErrorMessage);
        }

        try
        {
            int routeId = await routeService.AddWithInitialFlightAsync(
                request.CompanyId,
                request.AirportId,
                request.RouteType,
                request.Interval,
                request.Start,
                request.End,
                request.Dep,
                request.Arr,
                request.Capacity,
                request.FlightNum,
                request.RunwayId,
                request.GateId);

            return this.Ok(routeId);
        }
        catch (InvalidOperationException conflictException)
        {
            return this.Conflict(conflictException.Message);
        }
    }

    [HttpGet("normalize-type")]
    public ActionResult<string> NormalizeFlightType([FromQuery] string? routeType)
    {
        return this.Ok(new { value = routeService.NormalizeFlightType(routeType) });
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

        return this.Ok(new { value = routeService.GetRelevantTime(route) });
    }
}

public sealed record AddRouteWithFlightRequest(
    int CompanyId,
    int AirportId,
    string RouteType,
    int Interval,
    DateTime Start,
    DateTime End,
    TimeOnly Dep,
    TimeOnly Arr,
    int Capacity,
    string FlightNum,
    int RunwayId,
    int GateId);

public sealed record RouteTimeRequest(string? RouteType, TimeOnly DepartureTime, TimeOnly ArrivalTime);