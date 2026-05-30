using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class RouteServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IRouteService
{
    private const string BaseUrl = "api/routes";

    public async Task<IEnumerable<Route>> GetAllRoutesAsync()
    {
        return await GetListAsync<Route>(BaseUrl);
    }

    public async Task<Route?> GetRouteByIdAsync(int routeId)
    {
        return await GetOptionalAsync<Route>($"{BaseUrl}/{routeId}");
    }

    public async Task<int> AddWithInitialFlightAsync(int companyId, int airportId, string routeType, int recurrenceInterval,
        DateTime startDate, DateTime endDate, TimeOnly departureTime, TimeOnly arrivalTime,
        int capacity, string flightNumber, int runwayId, int gateId)
    {
        var payload = new
        {
            CompanyId = companyId,
            AirportId = airportId,
            RouteType = routeType,
            RecurrenceInterval = recurrenceInterval,
            StartDate = startDate,
            EndDate = endDate,
            DepartureTime = departureTime,
            ArrivalTime = arrivalTime,
            Capacity = capacity,
            FlightNumber = flightNumber,
            RunwayId = runwayId,
            GateId = gateId
        };
        return await PostForResultAsync<object, int>($"{BaseUrl}/add-with-flight", payload);
    }

    public string NormalizeFlightType(string? routeType)
    {
        if (string.IsNullOrEmpty(routeType)) return "One-Way";
        return string.Equals(routeType, "RoundTrip", StringComparison.OrdinalIgnoreCase) ? "Round-Trip" : "One-Way";
    }

    public string GetRelevantTime(Route? route)
    {
        if (route is null) return string.Empty;
        return route.DepartureTime.ToString("HH:mm");
    }
}
