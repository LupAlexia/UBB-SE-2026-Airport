using System;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class CancellationServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), ICancellationService
{
    private const string BaseUrl = "api/flightticket";

    public (bool CanCancel, string Reason) CanCancelTicket(FlightTicket ticket)
    {
        if (ticket is null) return (false, "Ticket not found.");
        if (string.Equals(ticket.Status, "Cancelled", StringComparison.OrdinalIgnoreCase)) return (false, "This ticket is already cancelled.");
        if (ticket.Flight is not null && ticket.Flight.Date < DateTime.Now) return (false, "This flight is already in the past and cannot be cancelled.");
        return (true, string.Empty);
    }

    public async Task CancelTicketAsync(int ticketId)
    {
        await PutAsync($"{BaseUrl}/{ticketId}/status", "Cancelled");
    }
}
