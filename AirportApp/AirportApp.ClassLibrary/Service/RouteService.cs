using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class RouteService(IRouteRepository routeRepository, IFlightRepository flightRepository,
    IGateRepository gateRepository, IRunwayRepository runwayRepository) : IRouteService
{
    public async Task<IEnumerable<Route>> GetAllRoutesAsync()
    {
        return await routeRepository.GetAsync();
    }

    public async Task<Route?> GetRouteByIdAsync(int routeId)
    {
        return await routeRepository.GetByIdAsync(routeId);
    }

    public async Task AddRouteAsync(Route route)
    {
        await routeRepository.AddAsync(route);
    }

    public async Task UpdateRouteAsync(Route route)
    {
        await routeRepository.UpdateAsync(route);
    }

    public async Task DeleteRouteAsync(int routeId)
    {
        await routeRepository.DeleteAsync(routeId);
    }

    public async Task AddWithInitialFlightAsync(Route route, Flight initialFlight)
    {
        var allFlights = await flightRepository.GetAsync();
        var sameDayFlights = allFlights.Where(f => f.Date.Date == initialFlight.Date.Date).ToList();

        int newStart = initialFlight.Route?.DepartureTime.Hour * 60 + initialFlight.Route?.DepartureTime.Minute ?? 0;
        int newEnd = initialFlight.Route?.ArrivalTime.Hour * 60 + initialFlight.Route?.ArrivalTime.Minute ?? 0;
        if (newEnd <= newStart) newEnd += 1440;

        int newGateId = initialFlight.Gate?.Id ?? 0;
        int newRunwayId = initialFlight.Runway?.Id ?? 0;

        newStart = route.DepartureTime.Hour * 60 + route.DepartureTime.Minute;
        newEnd = route.ArrivalTime.Hour * 60 + route.ArrivalTime.Minute;
        if (newEnd <= newStart) newEnd += 1440;

        foreach (var existingFlight in sameDayFlights)
        {
            if (existingFlight.Route is null) continue;

            bool sameGate = existingFlight.Gate?.Id == newGateId;
            bool sameRunway = existingFlight.Runway?.Id == newRunwayId;

            if (!sameGate && !sameRunway) continue;

            int existStart = existingFlight.Route.DepartureTime.Hour * 60 + existingFlight.Route.DepartureTime.Minute;
            int existEnd = existingFlight.Route.ArrivalTime.Hour * 60 + existingFlight.Route.ArrivalTime.Minute;
            if (existEnd <= existStart) existEnd += 1440;

            bool overlaps = newStart < existEnd && existStart < newEnd;
            if (overlaps)
            {
                string resource = sameGate ? "gate" : "runway";
                throw new InvalidOperationException($"Time conflict: the {resource} is already occupied during the requested time slot.");
            }
        }

        int routeId = await routeRepository.AddAsync(route);
        initialFlight.Route = new Route { Id = routeId };
        await flightRepository.AddAsync(initialFlight);
    }

    public string NormalizeFlightType(string? routeType)
    {
        if (string.IsNullOrWhiteSpace(routeType)) return "-";
        string upper = routeType.Trim().ToUpperInvariant();
        if (upper.StartsWith("ARR") || upper.StartsWith("ARRIVAL")) return "ARR";
        if (upper.StartsWith("DEP") || upper.StartsWith("DEPARTURE")) return "DEP";
        return upper;
    }

    public string GetRelevantTime(string? routeType, TimeOnly departureTime, TimeOnly arrivalTime)
    {
        if (routeType is null) return "-";
        string normalized = NormalizeFlightType(routeType);
        if (normalized == "ARR") return arrivalTime.ToString("HH:mm");
        return departureTime.ToString("HH:mm");
    }
}
