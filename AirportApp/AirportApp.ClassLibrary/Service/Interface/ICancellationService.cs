using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface ICancellationService
{
    (bool CanCancel, string Reason) CanCancelTicket(FlightTicket ticket);
    Task CancelTicketAsync(int ticketId);
}
