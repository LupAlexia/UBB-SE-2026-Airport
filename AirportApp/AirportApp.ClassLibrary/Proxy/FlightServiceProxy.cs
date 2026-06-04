using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class FlightServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IFlightService
{
    private const string BaseUrl = "api/flights";

    public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
    {
        var dtos = await GetListAsync<FlightDTO>(BaseUrl);
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<Flight?> GetFlightByIdAsync(int flightId)
    {
        var dto = await GetOptionalAsync<FlightDTO>($"{BaseUrl}/{flightId}");
        return dto is null ? null : MapToEntity(dto);
    }

    public async Task<IEnumerable<Flight>> GetFlightsByRouteIdAsync(int routeId)
    {
        var dtos = await GetListAsync<FlightDTO>($"{BaseUrl}/by-route/{routeId}");
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<int> AddFlightAsync(string flightNumber, int routeId, DateTime date, int runwayId, int gateId)
    {
        var payload = new { FlightNumber = flightNumber, RouteId = routeId, Date = date, RunwayId = runwayId, GateId = gateId };
        return await PostForResultAsync<object, int>(BaseUrl, payload);
    }

    public async Task UpdateFlightAsync(int flightId, DateTime? date, string? flightNumber, int? runwayId, int? gateId)
    {
        var payload = new { Date = date, FlightNumber = flightNumber, RunwayId = runwayId, GateId = gateId };
        await PutAsync($"{BaseUrl}/{flightId}", payload);
    }

    public async Task DeleteFlightAsync(int flightId)
    {
        await DeleteAsync($"{BaseUrl}/{flightId}");
    }

    public static Flight MapToEntity(FlightDTO dto)
    {
        var flight = new Flight
        {
            Id = dto.id,
            Date = dto.date,
            FlightNumber = dto.flightNumber,
            Gate = new Gate { Id = dto.gateId },
            Runway = new Runway { Id = dto.runwayId }
        };

        if (dto.route is not null)
        {
            flight.Route = RouteServiceProxy.MapToEntity(dto.route);
        }
        else
        {
            flight.Route = new Route { Id = dto.routeId };
        }

        return flight;
    }

    public static FlightDTO MapToDto(Flight flight)
    {
        var routeDto = flight.Route is not null ? RouteServiceProxy.MapToDto(flight.Route) : null;
        return new FlightDTO(flight.Id, flight.Route?.Id ?? 0, flight.Gate?.Id ?? 0, flight.Runway?.Id ?? 0, flight.Date, flight.FlightNumber, routeDto);
    }
}
