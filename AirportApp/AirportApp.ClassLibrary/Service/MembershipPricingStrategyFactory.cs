using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

// Members get the discount strategy (tied to their membership), everyone else
// (including guests with no user) gets the plain one.
public class MembershipPricingStrategyFactory : IMembershipPricingStrategyFactory
{
    public IMembershipPricingStrategy Create(Customer? customer)
    {
        if (customer?.Membership != null)
        {
            return new MembershipDiscountPricingStrategy(customer.Membership);
        }

        return new StandardPricingStrategy();
    }
}
