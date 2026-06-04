using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

// No membership -> no discounts. Just the base fare plus every add-on at full price.
public class StandardPricingStrategy : IMembershipPricingStrategy
{
    public float CalculateTicketTotal(FlightTicket ticket)
    {
        float total = ticket.Price;

        foreach (AddOn addon in ticket.SelectedAddOns)
        {
            total += addon.BasePrice;
        }

        return total;
    }
}
