using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IFlightRouteService
{
    Task<IEnumerable<Flight>> GetAllFlightsAsync();
    Task<Flight?> GetFlightByIdAsync(int flightId);
    Task DeleteFlightUsingIdAsync(int flightId);
    Task<IEnumerable<Route>> GetAllRoutesAsync();
    Task<Route?> GetRouteByIdAsync(int routeId);
    Task AddFlightToRouteAsync(Flight newFlight, IEnumerable<Flight> existingFlights);
    Task CreateFlightWithScheduleAsync(int routeId, int companyId, int airportId, string routeType,
        DateOnly startDate, DateOnly endDate, TimeOnly departureTime, TimeOnly arrivalTime,
        int capacity, int gateId, int runwayId, string recurrenceType, string customInterval);
    Task<IEnumerable<FlightSummary>> GetAllFlightsWithDetailsAsync();
    Task<IEnumerable<Flight>> GetFlightsByCompanyIdAsync(int companyId);
    Task<string> GetDestinationTextAsync(int routeId);
    Task<FlightSummary> BuildFlightSummaryAsync(Flight flight);
    Task<IEnumerable<Flight>> SearchFlightsAsync(string query);
    Task<IEnumerable<Flight>> SearchFlightsByNumberAsync(string flightNumber);
}
