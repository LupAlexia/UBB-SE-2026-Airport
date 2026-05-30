using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class FlightRouteServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IFlightRouteService
{
    private const string BaseUrl = "api/flightroutes";

    public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
    {
        return await GetListAsync<Flight>($"{BaseUrl}/flights");
    }

    public async Task<Flight?> GetFlightByIdAsync(int flightId)
    {
        return await GetOptionalAsync<Flight>($"{BaseUrl}/flights/{flightId}");
    }

    public async Task DeleteFlightUsingIdAsync(int flightId)
    {
        await DeleteAsync($"{BaseUrl}/flights/{flightId}");
    }

    public async Task<IEnumerable<Route>> GetAllRoutesAsync()
    {
        return await GetListAsync<Route>($"{BaseUrl}/routes");
    }

    public async Task<Route?> GetRouteByIdAsync(int routeId)
    {
        return await GetOptionalAsync<Route>($"{BaseUrl}/routes/{routeId}");
    }

    public async Task<int> AddFlightToRouteAsync(int companyId, int airportId, string routeType, int recurrenceInterval,
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
        return await PostForResultAsync<object, int>($"{BaseUrl}/add-flight", payload);
    }

    public Task CreateFlightWithScheduleAsync(int companyId, string? routeTypeDisplayName, int airportId, int capacity,
        TimeSpan departureOffset, TimeSpan arrivalOffset, bool isRecurrent,
        DateTime? startDate, DateTime? endDate, DateTime? singleDate,
        string recurrenceType, string customDaysText, int runwayId, int gateId,
        Func<int, string> flightCodeGenerator)
    {
        throw new NotSupportedException("CreateFlightWithScheduleAsync with a delegate parameter is not supported over HTTP.");
    }

    public async Task<IEnumerable<Flight>> GetAllFlightsWithDetailsAsync()
    {
        return await GetListAsync<Flight>($"{BaseUrl}/flights-details");
    }

    public async Task<IEnumerable<Flight>> GetFlightsByCompanyIdAsync(int companyId)
    {
        return await GetListAsync<Flight>($"{BaseUrl}/company/{companyId}/flights");
    }

    public async Task<string> GetDestinationTextAsync(Flight flight)
    {
        return await PostForResultAsync<Flight, string>($"{BaseUrl}/destination-text", flight);
    }

    public async Task<FlightSummary> BuildFlightSummaryAsync(Flight flight, string crewText)
    {
        return await PostForResultAsync<Flight, FlightSummary>($"{BaseUrl}/summary?crewText={Uri.EscapeDataString(crewText)}", flight);
    }

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(IEnumerable<Flight> flights, string query)
    {
        return await PostForResultAsync<IEnumerable<Flight>, IEnumerable<Flight>>($"{BaseUrl}/search?query={Uri.EscapeDataString(query)}", flights);
    }

    public async Task<IEnumerable<Flight>> SearchFlightsByNumberAsync(IEnumerable<Flight> flights, string query)
    {
        return await PostForResultAsync<IEnumerable<Flight>, IEnumerable<Flight>>($"{BaseUrl}/search-by-number?query={Uri.EscapeDataString(query)}", flights);
    }
}
