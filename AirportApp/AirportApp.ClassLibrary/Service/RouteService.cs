using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class RouteService(
    IRouteRepository routeRepository,
    IFlightRepository flightRepository,
    ICompanyRepository companyRepository,
    IAirportRepository airportRepository) : IRouteService
{
    private const int MinutesInADay = 1440;
    private const int MinutesInAnHour = 60;
    private const string ArrivalCode = "ARR";
    private const string ArrivalFullName = "ARRIVAL";
    private const string DepartureCode = "DEP";
    private const string DepartureFullName = "DEPARTURE";
    private const string EmptyFieldPlaceholder = "-";
    private const string TimeFormat = "HH:mm";

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

    public async Task<int> AddWithInitialFlightAsync(
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
        IEnumerable<Flight> allFlights = await flightRepository.GetAsync();
        List<Flight> sameDayFlights = new List<Flight>();

        foreach (Flight flight in allFlights)
        {
            if (flight.Date.Date == startDate.Date)
            {
                sameDayFlights.Add(flight);
            }
        }

        foreach (Flight existingFlight in sameDayFlights)
        {
            if (existingFlight.Gate?.Id == gateId || existingFlight.Runway?.Id == runwayId)
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
                        throw new InvalidOperationException("Resource Conflict: The selected Gate or Runway is already occupied during this time.");
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

        Flight initialFlight = new Flight
        {
            Route = new Route { Id = routeId },
            Date = startDate,
            FlightNumber = flightNumber,
            Runway = new Runway { Id = runwayId },
            Gate = new Gate { Id = gateId }
        };

        await flightRepository.AddAsync(initialFlight);
        return routeId;
    }

    public async Task<Route?> GetRouteByIdAsync(int routeId)
    {
        return await routeRepository.GetByIdAsync(routeId);
    }

    public async Task<IEnumerable<Route>> GetAllRoutesAsync()
    {
        return await routeRepository.GetAsync();
    }

    public string NormalizeFlightType(string? routeType)
    {
        if (string.IsNullOrWhiteSpace(routeType))
        {
            return EmptyFieldPlaceholder;
        }

        string upperCaseType = routeType.Trim().ToUpperInvariant();

        if (upperCaseType.StartsWith(ArrivalCode) || upperCaseType.StartsWith(ArrivalFullName))
        {
            return ArrivalCode;
        }

        if (upperCaseType.StartsWith(DepartureCode) || upperCaseType.StartsWith(DepartureFullName))
        {
            return DepartureCode;
        }

        return upperCaseType;
    }

    public string GetRelevantTime(Route? route)
    {
        if (route == null)
        {
            return EmptyFieldPlaceholder;
        }

        string normalizedType = NormalizeFlightType(route.RouteType);

        if (normalizedType == ArrivalCode)
        {
            return route.ArrivalTime.ToString(TimeFormat);
        }
        return route.DepartureTime.ToString(TimeFormat);
    }
}
