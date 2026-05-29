using System;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class CancellationService(IFlightTicketRepository ticketRepository) : ICancellationService
{
    public (bool CanCancel, string Reason) CanCancelTicket(FlightTicket ticket)
    {
        if (ticket == null)
            return (false, "Ticket not found.");
        if (string.Equals(ticket.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            return (false, "This ticket is already cancelled.");
        if (ticket.Flight != null && ticket.Flight.Date < DateTime.Now)
            return (false, "This flight is already in the past and cannot be cancelled.");
        return (true, string.Empty);
    }

    public async Task CancelTicketAsync(int ticketId)
    {
        await ticketRepository.UpdateStatusAsync(ticketId, "Cancelled");
    }
}
