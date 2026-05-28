using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IRouteService
{
    Task<IEnumerable<Route>> GetAllRoutesAsync();
    Task<Route?> GetRouteByIdAsync(int routeId);
    Task AddRouteAsync(Route route);
    Task UpdateRouteAsync(Route route);
    Task DeleteRouteAsync(int routeId);
    Task AddWithInitialFlightAsync(Route route, Flight initialFlight);
    string NormalizeFlightType(string? routeType);
    string GetRelevantTime(string? routeType, TimeOnly departureTime, TimeOnly arrivalTime);
}
