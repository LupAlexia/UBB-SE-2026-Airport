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

    private const int MinutesInADay = 1440;
    private const int MinutesInAnHour = 60;
    private const string ArrivalText = "Arrival";
    private const string ArrivalCode = "ARR";
    private const string DepartureCode = "DEP";
    private const string FlightDateTimeFormat = "dd.MM.yyyy HH:mm";
    private const string EmptyFieldPlaceholder = "-";

    private const int DailyIntervalDays = 1;
    private const int WeeklyIntervalDays = 7;
    private const int MonthlyIntervalDays = 30;

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

    public async Task CreateFlightWithScheduleAsync(int companyId, string? routeTypeDisplayName, int airportId, int capacity,
        TimeSpan departureOffset, TimeSpan arrivalOffset, bool isRecurrent,
        DateTime? startDate, DateTime? endDate, DateTime? singleDate,
        string recurrenceType, string customDaysText, int runwayId, int gateId,
        Func<int, string> flightCodeGenerator)
    {
        if (companyId <= 0)
        {
            throw new InvalidOperationException("A company must be selected before adding a flight.");
        }
        if (airportId <= 0 || runwayId <= 0 || gateId <= 0)
        {
            throw new InvalidOperationException("Please ensure all required fields are populated.");
        }
        if (capacity <= 0)
        {
            throw new InvalidOperationException("The provided capacity value is invalid.");
        }

        string routeType = routeTypeDisplayName == ArrivalText ? ArrivalCode : DepartureCode;

        DateTime start = isRecurrent ? startDate?.Date ?? DateTime.Today : singleDate?.Date ?? DateTime.Today;
        DateTime end = isRecurrent ? endDate?.Date ?? start : start;

        if (isRecurrent && end < start)
        {
            throw new InvalidOperationException("The end date must be after the start date.");
        }

        int interval = 0;
        if (isRecurrent)
        {
            interval = recurrenceType switch
            {
                nameof(RecurrenceType.Daily) => DailyIntervalDays,
                nameof(RecurrenceType.Weekly) => WeeklyIntervalDays,
                nameof(RecurrenceType.Monthly) => MonthlyIntervalDays,
                nameof(RecurrenceType.Custom) => ParseCustomInterval(customDaysText),
                _ => throw new InvalidOperationException("A recurrence type is required for recurrent flights.")
            };
        }

        TimeOnly departureTime = TimeOnly.FromTimeSpan(departureOffset);
        TimeOnly arrivalTime = TimeOnly.FromTimeSpan(arrivalOffset);

        if (departureTime == arrivalTime)
        {
            throw new InvalidOperationException("Arrival time cannot be identical to departure time.");
        }

        string flightNumber = flightCodeGenerator(companyId);
        await AddFlightToRouteAsync(companyId, airportId, routeType, interval, start, end, departureTime, arrivalTime, capacity, flightNumber, runwayId, gateId);
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

    public Task<string> GetDestinationTextAsync(Flight flight)
    {
        if (flight.Route == null || flight.Route.Airport == null)
        {
            return Task.FromResult(EmptyFieldPlaceholder);
        }

        return Task.FromResult($"{flight.Route.Airport.Code} - {flight.Route.Airport.Name}");
    }

    public Task<FlightSummary> BuildFlightSummaryAsync(Flight flight, string crewText)
    {
        string destinationText = flight.Route?.Airport != null ? $"{flight.Route.Airport.Code} - {flight.Route.Airport.Name}" : EmptyFieldPlaceholder;
        return Task.FromResult(new FlightSummary
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber ?? string.Empty,
            DateText = flight.Date.ToString(FlightDateTimeFormat),
            DestinationText = destinationText,
            RunwayText = flight.Runway?.Name ?? EmptyFieldPlaceholder,
            GateText = flight.Gate?.Name ?? EmptyFieldPlaceholder,
            CrewText = crewText
        });
    }

    public Task<IEnumerable<Flight>> SearchFlightsAsync(IEnumerable<Flight> flights, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Task.FromResult(flights);
        }

        List<Flight> matching = new List<Flight>();
        foreach (Flight flight in flights)
        {
            if (IsFlightMatch(flight, query))
            {
                matching.Add(flight);
            }
        }

        return Task.FromResult<IEnumerable<Flight>>(matching);
    }

    public Task<IEnumerable<Flight>> SearchFlightsByNumberAsync(IEnumerable<Flight> flights, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Task.FromResult(flights);
        }

        string queryLower = query.ToLowerInvariant();
        List<Flight> matching = new List<Flight>();
        foreach (Flight flight in flights)
        {
            if (flight.FlightNumber != null && flight.FlightNumber.ToLowerInvariant().Contains(queryLower))
            {
                matching.Add(flight);
            }
        }

        return Task.FromResult<IEnumerable<Flight>>(matching);
    }

    private static int ParseCustomInterval(string? customDaysText)
    {
        if (string.IsNullOrWhiteSpace(customDaysText))
        {
            throw new InvalidOperationException("Custom days are required when recurrence type is Custom.");
        }

        if (!int.TryParse(customDaysText, out int custom) || custom <= 0)
        {
            throw new InvalidOperationException("Invalid custom interval.");
        }

        return custom;
    }

    private bool IsFlightMatch(Flight flight, string query)
    {
        string queryLower = query.ToLowerInvariant();

        if (flight.FlightNumber != null && flight.FlightNumber.ToLowerInvariant().Contains(queryLower))
        {
            return true;
        }

        if (flight.Date.ToString(FlightDateTimeFormat).ToLowerInvariant().Contains(queryLower))
        {
            return true;
        }

        string destination = (flight.Route?.Airport != null ? $"{flight.Route.Airport.Code} - {flight.Route.Airport.Name}" : EmptyFieldPlaceholder).ToLowerInvariant();
        if (destination.Contains(queryLower))
        {
            return true;
        }

        if (flight.Runway?.Name != null && flight.Runway.Name.ToLowerInvariant().Contains(queryLower))
        {
            return true;
        }

        if (flight.Gate?.Name != null && flight.Gate.Name.ToLowerInvariant().Contains(queryLower))
        {
            return true;
        }

        return false;
    }
}
