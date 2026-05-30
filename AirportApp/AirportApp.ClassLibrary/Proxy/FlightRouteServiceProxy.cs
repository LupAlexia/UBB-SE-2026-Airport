using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class FlightRouteServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IFlightRouteService
{
    private const string BaseUrl = "api/flightroutes";

    public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
    {
        var dtos = await GetListAsync<FlightDTO>($"{BaseUrl}/flights");
        return dtos.Select(FlightServiceProxy.MapToEntity).ToList();
    }

    public async Task<Flight?> GetFlightByIdAsync(int flightId)
    {
        var dto = await GetOptionalAsync<FlightDTO>($"{BaseUrl}/flights/{flightId}");
        return dto is null ? null : FlightServiceProxy.MapToEntity(dto);
    }

    public async Task DeleteFlightUsingIdAsync(int flightId)
    {
        await DeleteAsync($"{BaseUrl}/flights/{flightId}");
    }

    public async Task<IEnumerable<Route>> GetAllRoutesAsync()
    {
        var dtos = await GetListAsync<RouteDTO>($"{BaseUrl}/routes");
        return dtos.Select(RouteServiceProxy.MapToEntity).ToList();
    }

    public async Task<Route?> GetRouteByIdAsync(int routeId)
    {
        var dto = await GetOptionalAsync<RouteDTO>($"{BaseUrl}/routes/{routeId}");
        return dto is null ? null : RouteServiceProxy.MapToEntity(dto);
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
        var dtos = await GetListAsync<FlightDTO>($"{BaseUrl}/flights-details");
        return dtos.Select(FlightServiceProxy.MapToEntity).ToList();
    }

    public async Task<IEnumerable<Flight>> GetFlightsByCompanyIdAsync(int companyId)
    {
        var dtos = await GetListAsync<FlightDTO>($"{BaseUrl}/company/{companyId}/flights");
        return dtos.Select(FlightServiceProxy.MapToEntity).ToList();
    }

    public async Task<string> GetDestinationTextAsync(Flight flight)
    {
        var dto = FlightServiceProxy.MapToDto(flight);
        return await PostForResultAsync<FlightDTO, string>($"{BaseUrl}/destination-text", dto);
    }

    public async Task<FlightSummary> BuildFlightSummaryAsync(Flight flight, string crewText)
    {
        var dto = FlightServiceProxy.MapToDto(flight);
        return await PostForResultAsync<FlightDTO, FlightSummary>($"{BaseUrl}/summary?crewText={Uri.EscapeDataString(crewText)}", dto);
    }

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(IEnumerable<Flight> flights, string query)
    {
        var dtos = flights.Select(FlightServiceProxy.MapToDto).ToList();
        var resultDtos = await PostForResultAsync<IEnumerable<FlightDTO>, IEnumerable<FlightDTO>>($"{BaseUrl}/search?query={Uri.EscapeDataString(query)}", dtos);
        return resultDtos.Select(FlightServiceProxy.MapToEntity).ToList();
    }

    public async Task<IEnumerable<Flight>> SearchFlightsByNumberAsync(IEnumerable<Flight> flights, string query)
    {
        var dtos = flights.Select(FlightServiceProxy.MapToDto).ToList();
        var resultDtos = await PostForResultAsync<IEnumerable<FlightDTO>, IEnumerable<FlightDTO>>($"{BaseUrl}/search-by-number?query={Uri.EscapeDataString(query)}", dtos);
        return resultDtos.Select(FlightServiceProxy.MapToEntity).ToList();
    }
}
