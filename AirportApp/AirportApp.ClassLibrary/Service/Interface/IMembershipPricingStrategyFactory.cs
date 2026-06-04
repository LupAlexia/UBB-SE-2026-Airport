using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

// Decides which pricing strategy a given customer should get.
public interface IMembershipPricingStrategyFactory
{
    IMembershipPricingStrategy Create(Customer? customer);
}
