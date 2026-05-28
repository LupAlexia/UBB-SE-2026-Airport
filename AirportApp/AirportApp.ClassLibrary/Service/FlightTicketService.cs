using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class FlightTicketService(IFlightTicketRepository flightTicketRepository, IAddOnRepository addOnRepository) : IFlightTicketService
{
    public async Task<IEnumerable<FlightTicket>> GetByUserIdAsync(int userId)
    {
        return await flightTicketRepository.GetByUserIdAsync(userId);
    }

    public async Task<int> AddAsync(FlightTicket flightTicket)
    {
        return await flightTicketRepository.AddAsync(flightTicket);
    }

    public async Task UpdateStatusAsync(int flightTicketId, string status)
    {
        await flightTicketRepository.UpdateStatusAsync(flightTicketId, status);
    }

    public async Task AddAddOnsAsync(int flightTicketId, IEnumerable<int> addOnIds)
    {
        await flightTicketRepository.AddAddOnsAsync(flightTicketId, addOnIds);
    }

    public async Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId)
    {
        return await flightTicketRepository.GetOccupiedSeatsAsync(flightId);
    }

    public async Task<bool> IsSeatAvailableAsync(int flightId, string seat)
    {
        return await flightTicketRepository.IsSeatAvailableAsync(flightId, seat);
    }

    public async Task<bool> SaveBatchWithAddOnsAsync(List<FlightTicket> tickets, List<List<int>> addOnIds)
    {
        return await flightTicketRepository.SaveBatchWithAddOnsAsync(tickets, addOnIds);
    }

    public async Task<IEnumerable<AddOn>> GetAddOnsByIdsAsync(IEnumerable<int> addOnIds)
    {
        return await addOnRepository.GetByIdsAsync(addOnIds);
    }
}
