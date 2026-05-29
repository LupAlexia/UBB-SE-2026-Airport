using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public enum RecurrenceType
{
    Daily,
    Weekly,
    Monthly,
    Custom
}

public class FlightRouteService(
    IFlightRepository flightRepository,
    IRouteRepository routeRepository,
    ICompanyRepository companyRepository,
    IAirportRepository airportRepository,
    IRunwayService runwayService,
    IGateService gateService,
    IAirportService airportService) : IFlightRouteService
{
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

    private bool CheckOverlappingTimes(TimeOnly startTime1, TimeOnly endTime1, TimeOnly startTime2, TimeOnly endTime2)
    {
        int startMinutes1 = (startTime1.Hour * MinutesInAnHour) + startTime1.Minute;
        int endMinutes1 = (endTime1.Hour * MinutesInAnHour) + endTime1.Minute;

        if (endMinutes1 <= startMinutes1)
        {
            endMinutes1 += MinutesInADay;
        }

        int startMinutes2 = (startTime2.Hour * MinutesInAnHour) + startTime2.Minute;
        int endMinutes2 = (endTime2.Hour * MinutesInAnHour) + endTime2.Minute;

        if (endMinutes2 <= startMinutes2)
        {
            endMinutes2 += MinutesInADay;
        }

        return startMinutes1 < endMinutes2 && startMinutes2 < endMinutes1;
    }

    public async Task<int> AddFlightToRouteAsync(
        int companyId,
        int airportId,
        string routeType,
        int recurrenceInterval,
        DateTime startDate,
        DateTime endDate,
        TimeOnly departureTime,
        TimeOnly arrivalTime,
        int capacity,
        string flightNumber,
        int runwayId,
        int gateId)
    {
        if (startDate > endDate)
        {
            throw new ArgumentException("The start date cannot be after the end date.");
        }

        if (capacity <= 0)
        {
            throw new ArgumentException("Capacity must be a positive number greater than 0.");
        }

        IEnumerable<Flight> allFlights = await flightRepository.GetAsync();

        foreach (Flight existingFlight in allFlights)
        {
            if (existingFlight.Date.Date == startDate.Date)
            {
                if (existingFlight.Gate.Id == gateId || existingFlight.Runway.Id == runwayId)
                {
                    Route? existingRoute = await routeRepository.GetByIdAsync(existingFlight.Route.Id);

                    if (existingRoute != null)
                    {
                        bool isTimeOverlap = CheckOverlappingTimes(
                            departureTime,
                            arrivalTime,
                            existingRoute.DepartureTime,
                            existingRoute.ArrivalTime);

                        if (isTimeOverlap)
                        {
                            if (existingFlight.Gate.Id == gateId)
                            {
                                throw new InvalidOperationException($"Gate Conflict: Gate {gateId} is occupied by Flight {existingFlight.FlightNumber}.");
                            }

                            if (existingFlight.Runway.Id == runwayId)
                            {
                                throw new InvalidOperationException($"Runway Conflict: Runway {runwayId} is occupied by Flight {existingFlight.FlightNumber}.");
                            }
                        }
                    }
                }
            }
        }

        Company? company = await companyRepository.GetByIdAsync(companyId);
        Airport? airport = await airportRepository.GetByIdAsync(airportId);

        Route newRoute = new Route
        {
            Company = company!,
            Airport = airport!,
            RouteType = routeType,
            RecurrenceInterval = recurrenceInterval,
            StartDate = DateOnly.FromDateTime(startDate),
            EndDate = DateOnly.FromDateTime(endDate),
            DepartureTime = departureTime,
            ArrivalTime = arrivalTime,
            Capacity = capacity
        };

        int routeId = await routeRepository.AddAsync(newRoute);

        DateTime flightFullDateTime = startDate.Date.Add(departureTime.ToTimeSpan());

        Flight initialFlight = new Flight
        {
            Route = new Route { Id = routeId },
            Date = flightFullDateTime,
            FlightNumber = flightNumber,
            Runway = new Runway { Id = runwayId },
            Gate = new Gate { Id = gateId }
        };

        await flightRepository.AddAsync(initialFlight);

        return routeId;
    }

    public async Task CreateFlightWithScheduleAsync(
        int companyId,
        string? routeTypeDisplayName,
        int airportId,
        int capacity,
        TimeSpan departureOffset,
        TimeSpan arrivalOffset,
        bool isRecurrent,
        DateTime? startDate,
        DateTime? endDate,
        DateTime? singleDate,
        string recurrenceType,
        string customDaysText,
        int runwayId,
        int gateId,
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
                nameof(RecurrenceType.Custom) => int.TryParse(customDaysText, out int custom) && custom > 0
                                                      ? custom
                                                      : throw new InvalidOperationException("Invalid custom interval."),
                _ => throw new InvalidOperationException("A recurrence type is required for recurrent flights.")
            };
        }

        TimeOnly depTime = TimeOnly.FromTimeSpan(departureOffset);
        TimeOnly arrTime = TimeOnly.FromTimeSpan(arrivalOffset);

        if (depTime == arrTime)
        {
            throw new InvalidOperationException("Arrival time cannot be identical to departure time.");
        }

        string flightNumber = flightCodeGenerator(companyId);
        await AddFlightToRouteAsync(companyId, airportId, routeType, interval, start, end, depTime, arrTime, capacity, flightNumber, runwayId, gateId);
    }

    public async Task<IEnumerable<Flight>> GetAllFlightsWithDetailsAsync()
    {
        List<Flight> flights = (await GetAllFlightsAsync()).ToList();

        foreach (Flight flight in flights)
        {
            if (flight.Runway != null && flight.Runway.Id > 0)
            {
                flight.Runway = await runwayService.GetRunwayByIdAsync(flight.Runway.Id);
            }

            if (flight.Gate != null && flight.Gate.Id > 0)
            {
                flight.Gate = await gateService.GetGateByIdAsync(flight.Gate.Id);
            }

            if (flight.Route != null && flight.Route.Id > 0)
            {
                flight.Route = await GetRouteByIdAsync(flight.Route.Id);

                if (flight.Route?.Airport != null && flight.Route.Airport.Id > 0)
                {
                    flight.Route.Airport = await airportService.GetAirportByIdAsync(flight.Route.Airport.Id);
                }
            }
        }

        return flights;
    }

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(IEnumerable<Flight> flights, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return flights;
        }

        List<Flight> matchingFlights = new List<Flight>();
        foreach (Flight flight in flights)
        {
            if (IsFlightMatch(flight, query))
            {
                matchingFlights.Add(flight);
            }
        }

        await Task.CompletedTask;
        return matchingFlights;
    }

    public async Task<IEnumerable<Flight>> SearchFlightsByNumberAsync(IEnumerable<Flight> flights, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return flights;
        }

        List<Flight> matchingFlights = new List<Flight>();
        foreach (Flight flight in flights)
        {
            if (flight.FlightNumber != null && flight.FlightNumber.ToLowerInvariant().Contains(query))
            {
                matchingFlights.Add(flight);
            }
        }

        await Task.CompletedTask;
        return matchingFlights;
    }

    private bool IsFlightMatch(Flight flight, string query)
    {
        if (flight.FlightNumber != null && flight.FlightNumber.ToLowerInvariant().Contains(query))
        {
            return true;
        }

        if (flight.Date.ToString(FlightDateTimeFormat).ToLowerInvariant().Contains(query))
        {
            return true;
        }

        string destination = GetDestinationText(flight).ToLowerInvariant();
        if (destination.Contains(query))
        {
            return true;
        }

        if (flight.Runway?.Name != null && flight.Runway.Name.ToLowerInvariant().Contains(query))
        {
            return true;
        }

        if (flight.Gate?.GateName != null && flight.Gate.GateName.ToLowerInvariant().Contains(query))
        {
            return true;
        }

        return false;
    }

    public async Task<Route?> GetRouteByIdAsync(int routeId)
    {
        return await routeRepository.GetByIdAsync(routeId);
    }

    public async Task<Flight?> GetFlightByIdAsync(int flightId)
    {
        return await flightRepository.GetByIdAsync(flightId);
    }

    public async Task<IEnumerable<Route>> GetAllRoutesAsync()
    {
        return await routeRepository.GetAsync();
    }

    public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
    {
        return await flightRepository.GetAsync();
    }

    public async Task DeleteFlightUsingIdAsync(int flightId)
    {
        if (flightId <= 0)
        {
            throw new ArgumentException("The provided flight Id is invalid.");
        }

        if (await GetFlightByIdAsync(flightId) == null)
        {
            throw new ArgumentException("A flight with the specified Id does not exist.");
        }

        await flightRepository.DeleteAsync(flightId);
    }

    public async Task<IEnumerable<Flight>> GetFlightsByCompanyIdAsync(int companyId)
    {
        IEnumerable<Route> allRoutes = await routeRepository.GetAsync();
        List<int> companyRouteIds = new List<int>();

        foreach (Route route in allRoutes)
        {
            if (route.Company.Id == companyId)
            {
                companyRouteIds.Add(route.Id);
            }
        }

        IEnumerable<Flight> allFlights = await flightRepository.GetAsync();
        List<Flight> filteredFlights = new List<Flight>();

        foreach (Flight flight in allFlights)
        {
            if (companyRouteIds.Contains(flight.Route.Id))
            {
                filteredFlights.Add(flight);
            }
        }

        return filteredFlights;
    }

    public string GetDestinationText(Flight flight)
    {
        if (flight.Route == null || flight.Route.Airport == null)
        {
            return EmptyFieldPlaceholder;
        }

        return $"{flight.Route.Airport.AirportCode} - {flight.Route.Airport.Name}";
    }

    public async Task<string> GetDestinationTextAsync(Flight flight)
    {
        await Task.CompletedTask;
        return GetDestinationText(flight);
    }

    public async Task<FlightSummary> BuildFlightSummaryAsync(Flight flight, string crewText)
    {
        await Task.CompletedTask;
        return new FlightSummary
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber ?? string.Empty,
            DateText = flight.Date.ToString(FlightDateTimeFormat),
            DestinationText = GetDestinationText(flight),
            RunwayText = flight.Runway?.Name ?? EmptyFieldPlaceholder,
            GateText = flight.Gate?.GateName ?? EmptyFieldPlaceholder,
            CrewText = crewText
        };
    }
}
