using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IFlightRouteService
{
    Task<IEnumerable<Flight>> GetAllFlightsAsync();
    Task<Flight?> GetFlightByIdAsync(int flightId);
    Task DeleteFlightUsingIdAsync(int flightId);
    Task<IEnumerable<Route>> GetAllRoutesAsync();
    Task<Route?> GetRouteByIdAsync(int routeId);
    Task<int> AddFlightToRouteAsync(int companyId, int airportId, string routeType, int recurrenceInterval,
        DateTime startDate, DateTime endDate, TimeOnly departureTime, TimeOnly arrivalTime,
        int capacity, string flightNumber, int runwayId, int gateId);
    Task CreateFlightWithScheduleAsync(int companyId, string? routeTypeDisplayName, int airportId, int capacity,
        TimeSpan departureOffset, TimeSpan arrivalOffset, bool isRecurrent,
        DateTime? startDate, DateTime? endDate, DateTime? singleDate,
        string recurrenceType, string customDaysText, int runwayId, int gateId,
        Func<int, string> flightCodeGenerator);
    Task<IEnumerable<Flight>> GetAllFlightsWithDetailsAsync();
    Task<IEnumerable<Flight>> GetFlightsByCompanyIdAsync(int companyId);
    Task<string> GetDestinationTextAsync(Flight flight);
    Task<FlightSummary> BuildFlightSummaryAsync(Flight flight, string crewText);
    Task<IEnumerable<Flight>> SearchFlightsAsync(IEnumerable<Flight> flights, string query);
    Task<IEnumerable<Flight>> SearchFlightsByNumberAsync(IEnumerable<Flight> flights, string query);
}
