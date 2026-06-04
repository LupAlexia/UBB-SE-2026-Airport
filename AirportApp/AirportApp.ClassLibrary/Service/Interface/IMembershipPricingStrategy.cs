using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

// A way of working out a ticket's final price. There's one version for plain
// customers and one for members; PricingService just uses whichever it gets.
public interface IMembershipPricingStrategy
{
    float CalculateTicketTotal(FlightTicket ticket);
}
