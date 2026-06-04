using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

// Customer has a membership, so apply its discounts:
//  - a percentage off the base fare
//  - a per-add-on percentage off (when the membership discounts that add-on)
public class MembershipDiscountPricingStrategy : IMembershipPricingStrategy
{
    private const float PercentageDivisor = 100.0f;
    private const float ZeroPrice = 0f;

    private readonly Membership membership;

    public MembershipDiscountPricingStrategy(Membership membership)
    {
        this.membership = membership;
    }

    public float CalculateTicketTotal(FlightTicket ticket)
    {
        // Base fare minus the membership's flight discount.
        float total = ticket.Price;
        total -= total * (membership.FlightDiscountPercentage / PercentageDivisor);

        // Add each add-on, taking off its own discount if this membership has one.
        foreach (AddOn addon in ticket.SelectedAddOns)
        {
            float discount = ResolveAddonDiscount(addon);
            total += addon.BasePrice - (addon.BasePrice * (discount / PercentageDivisor));
        }

        return total;
    }

    // Find the discount this membership gives for one add-on (0 if there isn't one).
    private float ResolveAddonDiscount(AddOn addon)
    {
        if (membership.AddonDiscounts == null)
        {
            return ZeroPrice;
        }

        foreach (MembershipAddonDiscount discount in membership.AddonDiscounts)
        {
            if (discount.AddOn != null && discount.AddOn.Id == addon.Id)
            {
                return discount.DiscountPercentage;
            }
        }

        return ZeroPrice;
    }
}
