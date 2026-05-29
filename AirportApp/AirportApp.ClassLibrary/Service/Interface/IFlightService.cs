using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IFlightService
{
    Task<IEnumerable<Flight>> GetAllFlightsAsync();
    Task<Flight?> GetFlightByIdAsync(int flightId);
    Task<IEnumerable<Flight>> GetFlightsByRouteIdAsync(int routeId);
    Task<int> AddFlightAsync(string flightNumber, int routeId, DateTime date, int runwayId, int gateId);
    Task UpdateFlightAsync(int flightId, DateTime? date, string? flightNumber, int? runwayId, int? gateId);
    Task DeleteFlightAsync(int flightId);
}
