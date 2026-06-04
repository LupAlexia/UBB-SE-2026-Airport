using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class PricingService : IPricingService
{
    private const float PricePerMinuteMultiplier = 1.25f;
    private const float MinimumFlightPrice = 40f;
    private const float ZeroPrice = 0f;

    // Hands us the right discount strategy for each customer (member vs. non-member).
    private readonly IMembershipPricingStrategyFactory pricingStrategyFactory;

    public PricingService(IMembershipPricingStrategyFactory pricingStrategyFactory)
    {
        this.pricingStrategyFactory = pricingStrategyFactory;
    }

    public Task<float> CalculateBasePriceAsync(Flight flight)
    {
        if (flight?.Route == null)
            return Task.FromResult(ZeroPrice);

        TimeSpan duration = flight.Route.ArrivalTime - flight.Route.DepartureTime;
        float calculatedPrice = (float)duration.TotalMinutes * PricePerMinuteMultiplier;
        return Task.FromResult(Math.Max(calculatedPrice, MinimumFlightPrice));
    }

    public Task<float> CalculateTotalPriceAsync(FlightTicket ticket)
    {
        // Pick the pricing rules based on the ticket's customer, then let that
        // strategy do the actual math (base fare + add-ons, with any discounts).
        IMembershipPricingStrategy pricingStrategy = pricingStrategyFactory.Create(ticket.User);
        float finalTotal = pricingStrategy.CalculateTicketTotal(ticket);
        return Task.FromResult(finalTotal);
    }

    public async Task<PriceBreakdown> CalculatePriceBreakdownAsync(Flight flight, Customer user, List<FlightTicket> tickets)
    {
        if (flight == null || tickets == null || tickets.Count == 0)
            return new PriceBreakdown();

        float basePrice = await CalculateBasePriceAsync(flight);
        float basePriceTotal = basePrice * tickets.Count;

        float addOnsWithoutMembership = tickets.Sum(ticket => ticket.SelectedAddOns.Sum(addOn => addOn.BasePrice));
        float totalWithoutMembership = basePriceTotal + addOnsWithoutMembership;

        float finalTotal = ZeroPrice;
        foreach (var ticket in tickets)
        {
            ticket.User = user;
            ticket.Price = basePrice;
            finalTotal += await CalculateTotalPriceAsync(ticket);
        }

        float membershipSavings = Math.Max(0, totalWithoutMembership - finalTotal);

        return new PriceBreakdown
        {
            BasePricePerPerson = basePrice,
            BasePriceTotal = basePriceTotal,
            AddOnsTotal = addOnsWithoutMembership,
            MembershipSavings = membershipSavings,
            FinalTotal = finalTotal
        };
    }
}
