using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IFlightRepository
{
    Task<IEnumerable<Flight>> GetAsync();
    Task<Flight?> GetByIdAsync(int flightId);
    Task<IEnumerable<Flight>> GetByRouteIdAsync(int routeId);
    Task<IEnumerable<Flight>> GetByRunwayIdAsync(int runwayId);
    Task<IEnumerable<Flight>> GetByGateIdAsync(int gateId);
    Task<IEnumerable<Flight>> GetByAirportIdAsync(int airportId);
    Task<int> AddAsync(Flight flight);
    Task UpdateAsync(Flight flight);
    Task DeleteAsync(int flightId);

    Task<IEnumerable<Flight>> SearchFlightsAsync(string location, string routeType, DateTime? date);
    Task<int> GetOccupiedSeatCountAsync(int flightId);

}