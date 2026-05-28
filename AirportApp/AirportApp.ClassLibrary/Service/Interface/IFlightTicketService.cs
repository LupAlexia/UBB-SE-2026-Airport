using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IFlightTicketService
{
    Task<IEnumerable<FlightTicket>> GetByUserIdAsync(int userId);
    Task<int> AddAsync(FlightTicket flightTicket);
    Task UpdateStatusAsync(int flightTicketId, string status);
    Task AddAddOnsAsync(int flightTicketId, IEnumerable<int> addOnIds);
    Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId);
    Task<bool> IsSeatAvailableAsync(int flightId, string seat);
    Task<bool> SaveBatchWithAddOnsAsync(List<FlightTicket> tickets, List<List<int>> addOnIds);
    Task<IEnumerable<AddOn>> GetAddOnsByIdsAsync(IEnumerable<int> addOnIds);
}
