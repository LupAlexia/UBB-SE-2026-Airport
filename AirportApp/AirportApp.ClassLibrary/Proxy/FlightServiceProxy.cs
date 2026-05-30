using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class FlightServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IFlightService
{
    private const string BaseUrl = "api/flights";

    public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
    {
        return await GetListAsync<Flight>(BaseUrl);
    }

    public async Task<Flight?> GetFlightByIdAsync(int flightId)
    {
        return await GetOptionalAsync<Flight>($"{BaseUrl}/{flightId}");
    }

    public async Task<IEnumerable<Flight>> GetFlightsByRouteIdAsync(int routeId)
    {
        return await GetListAsync<Flight>($"{BaseUrl}/route/{routeId}");
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
}
