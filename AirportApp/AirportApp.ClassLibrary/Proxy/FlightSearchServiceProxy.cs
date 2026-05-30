using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class FlightSearchServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IFlightSearchService
{
    private const string BaseUrl = "api/flightsearch";

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(string location, bool isDeparture, DateTime? date, int? passengers)
    {
        string dateParam = date.HasValue ? $"&date={date.Value:o}" : "";
        string passengerParam = passengers.HasValue ? $"&passengers={passengers.Value}" : "";
        return await GetListAsync<Flight>($"{BaseUrl}?location={Uri.EscapeDataString(location)}&isDeparture={isDeparture}{dateParam}{passengerParam}");
    }

    public int? ParsePassengerCount(string input)
    {
        return int.TryParse(input, out int result) ? result : null;
    }

    public async Task<Flight?> GetFlightByIdAsync(int id)
    {
        return await GetOptionalAsync<Flight>($"{BaseUrl}/{id}");
    }

    public async Task<IEnumerable<Flight>> GetFlightsByRouteAsync(string location, string routeType, DateTime? date)
    {
        string dateParam = date.HasValue ? $"&date={date.Value:o}" : "";
        return await GetListAsync<Flight>($"{BaseUrl}/by-route?location={Uri.EscapeDataString(location)}&routeType={Uri.EscapeDataString(routeType)}{dateParam}");
    }

    public async Task<int> GetOccupiedSeatCountAsync(int flightId)
    {
        return await GetRequiredAsync<int>($"{BaseUrl}/{flightId}/occupied-seat-count");
    }
}
