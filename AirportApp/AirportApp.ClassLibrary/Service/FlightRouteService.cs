using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class FlightRouteService(
    IFlightRepository flightRepository,
    IRouteRepository routeRepository,
    IAirportRepository airportRepository,
    IGateRepository gateRepository,
    IRunwayRepository runwayRepository) : IFlightRouteService
{
    public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
    {
        return await flightRepository.GetAsync();
    }

    public async Task<Flight?> GetFlightByIdAsync(int flightId)
    {
        return await flightRepository.GetByIdAsync(flightId);
    }

    public async Task DeleteFlightUsingIdAsync(int flightId)
    {
        await flightRepository.DeleteAsync(flightId);
    }

    public async Task<IEnumerable<Route>> GetAllRoutesAsync()
    {
        return await routeRepository.GetAsync();
    }

    public async Task<Route?> GetRouteByIdAsync(int routeId)
    {
        return await routeRepository.GetByIdAsync(routeId);
    }

    public async Task AddFlightToRouteAsync(Flight newFlight, IEnumerable<Flight> existingFlights)
    {
        if (newFlight.Route is null)
            throw new ArgumentException("Flight must have a route.");

        var route = await routeRepository.GetByIdAsync(newFlight.Route.Id);
        if (route is null)
            throw new InvalidOperationException($"Route with ID {newFlight.Route.Id} not found.");

        int newGateId = newFlight.Gate?.Id ?? 0;
        int newRunwayId = newFlight.Runway?.Id ?? 0;

        int newStart = route.DepartureTime.Hour * 60 + route.DepartureTime.Minute;
        int newEnd = route.ArrivalTime.Hour * 60 + route.ArrivalTime.Minute;
        if (newEnd <= newStart) newEnd += 1440;

        var sameDayFlights = existingFlights.Where(f => f.Date.Date == newFlight.Date.Date).ToList();

        foreach (var existing in sameDayFlights)
        {
            if (existing.Route is null) continue;

            bool sameGate = existing.Gate?.Id == newGateId;
            bool sameRunway = existing.Runway?.Id == newRunwayId;

            if (!sameGate && !sameRunway) continue;

            int existStart = existing.Route.DepartureTime.Hour * 60 + existing.Route.DepartureTime.Minute;
            int existEnd = existing.Route.ArrivalTime.Hour * 60 + existing.Route.ArrivalTime.Minute;
            if (existEnd <= existStart) existEnd += 1440;

            bool overlaps = newStart < existEnd && existStart < newEnd;
            if (overlaps)
            {
                if (sameGate)
                    throw new InvalidOperationException($"Gate conflict: Gate {newGateId} is already occupied during that time slot.");
                if (sameRunway)
                    throw new InvalidOperationException($"Runway conflict: Runway {newRunwayId} is already occupied during that time slot.");
            }
        }

        await flightRepository.AddAsync(newFlight);
    }

    public async Task CreateFlightWithScheduleAsync(int routeId, int companyId, int airportId, string routeType,
        DateOnly startDate, DateOnly endDate, TimeOnly departureTime, TimeOnly arrivalTime,
        int capacity, int gateId, int runwayId, string recurrenceType, string customInterval)
    {
        int intervalDays = recurrenceType switch
        {
            "Daily" => 1,
            "Weekly" => 7,
            "Monthly" => 30,
            "Custom" => int.TryParse(customInterval, out int ci) ? ci : throw new ArgumentException("Invalid custom interval."),
            _ => throw new ArgumentException($"Unknown recurrence type: {recurrenceType}")
        };

        var company = new Company { Id = companyId };
        var airport = new Airport { Id = airportId };
        var gate = new Gate { Id = gateId };
        var runway = new Runway { Id = runwayId };

        var route = new Route
        {
            Company = company,
            Airport = airport,
            RouteType = routeType,
            StartDate = startDate,
            EndDate = endDate,
            DepartureTime = departureTime,
            ArrivalTime = arrivalTime,
            Capacity = capacity,
            RecurrenceInterval = intervalDays
        };

        int newRouteId = await routeRepository.AddAsync(route);
        route.Id = newRouteId;

        var allFlights = (await flightRepository.GetAsync()).ToList();

        int start = departureTime.Hour * 60 + departureTime.Minute;
        int end = arrivalTime.Hour * 60 + arrivalTime.Minute;
        if (end <= start) end += 1440;

        var current = startDate;
        while (current <= endDate)
        {
            var flightDate = current.ToDateTime(departureTime);
            var sameDayFlights = allFlights.Where(f => f.Date.Date == flightDate.Date).ToList();

            bool hasConflict = false;
            foreach (var existing in sameDayFlights)
            {
                if (existing.Route is null) continue;
                bool sameGate = existing.Gate?.Id == gateId;
                bool sameRunway = existing.Runway?.Id == runwayId;
                if (!sameGate && !sameRunway) continue;

                int existStart = existing.Route.DepartureTime.Hour * 60 + existing.Route.DepartureTime.Minute;
                int existEnd = existing.Route.ArrivalTime.Hour * 60 + existing.Route.ArrivalTime.Minute;
                if (existEnd <= existStart) existEnd += 1440;

                if (start < existEnd && existStart < end)
                {
                    hasConflict = true;
                    break;
                }
            }

            if (!hasConflict)
            {
                var flight = new Flight
                {
                    Route = route,
                    Gate = gate,
                    Runway = runway,
                    Date = flightDate,
                    FlightNumber = string.Empty
                };
                await flightRepository.AddAsync(flight);
            }

            current = current.AddDays(intervalDays);
        }
    }

    public async Task<IEnumerable<FlightSummary>> GetAllFlightsWithDetailsAsync()
    {
        var flights = await flightRepository.GetAsync();
        var summaries = new List<FlightSummary>();
        foreach (var flight in flights)
        {
            summaries.Add(await BuildFlightSummaryAsync(flight));
        }
        return summaries;
    }

    public async Task<IEnumerable<Flight>> GetFlightsByCompanyIdAsync(int companyId)
    {
        var allFlights = await flightRepository.GetAsync();
        return allFlights.Where(f => f.Route?.Company?.Id == companyId).ToList();
    }

    public async Task<string> GetDestinationTextAsync(int routeId)
    {
        var route = await routeRepository.GetByIdAsync(routeId);
        if (route?.Airport is null) return "Unknown";
        return route.Airport.Name ?? route.Airport.City ?? "Unknown";
    }

    public async Task<FlightSummary> BuildFlightSummaryAsync(Flight flight)
    {
        string destination = flight.Route is not null
            ? await GetDestinationTextAsync(flight.Route.Id)
            : "Unknown";

        var employeeIds = new List<int>();
        return new FlightSummary
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            DateText = flight.Date.ToString("yyyy-MM-dd HH:mm"),
            DestinationText = destination,
            RunwayText = flight.Runway?.Name ?? "-",
            GateText = flight.Gate?.GateName ?? "-",
            CrewText = "Unassigned"
        };
    }

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(string query)
    {
        var allFlights = await flightRepository.GetAsync();
        if (string.IsNullOrWhiteSpace(query)) return allFlights;

        string lowerQuery = query.ToLowerInvariant();
        return allFlights.Where(f =>
            f.FlightNumber.ToLowerInvariant().Contains(lowerQuery) ||
            (f.Route?.RouteType?.ToLowerInvariant().Contains(lowerQuery) ?? false) ||
            (f.Route?.Airport?.City?.ToLowerInvariant().Contains(lowerQuery) ?? false)
        ).ToList();
    }

    public async Task<IEnumerable<Flight>> SearchFlightsByNumberAsync(string flightNumber)
    {
        var allFlights = await flightRepository.GetAsync();
        if (string.IsNullOrWhiteSpace(flightNumber)) return allFlights;
        string lower = flightNumber.ToLowerInvariant();
        return allFlights.Where(f => f.FlightNumber.ToLowerInvariant().Contains(lower)).ToList();
    }
}
