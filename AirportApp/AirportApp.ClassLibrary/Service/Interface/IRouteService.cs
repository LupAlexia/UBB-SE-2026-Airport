using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IRouteService
{
    Task<IEnumerable<Route>> GetAllRoutesAsync();
    Task<Route?> GetRouteByIdAsync(int routeId);
    Task<int> AddWithInitialFlightAsync(int companyId, int airportId, string routeType, int recurrenceInterval,
        DateTime startDate, DateTime endDate, TimeOnly departureTime, TimeOnly arrivalTime,
        int capacity, string flightNumber, int runwayId, int gateId);
    string NormalizeFlightType(string? routeType);
    string GetRelevantTime(Route? route);
}
