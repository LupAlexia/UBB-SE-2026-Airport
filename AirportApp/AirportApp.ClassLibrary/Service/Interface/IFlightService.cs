using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IFlightService
{
    Task<IEnumerable<Flight>> GetAllFlightsAsync();
    Task<Flight?> GetFlightByIdAsync(int flightId);
    Task AddFlightAsync(Flight flight);
    Task UpdateFlightAsync(Flight flight);
    Task DeleteFlightAsync(int flightId);
    Task<IEnumerable<Flight>> SearchFlightsAsync(string location, string routeType, DateTime? date);
    Task<int> GetOccupiedSeatCountAsync(int flightId);
    Task<IEnumerable<Flight>> GetFlightsByRouteIdAsync(int routeId);
}
