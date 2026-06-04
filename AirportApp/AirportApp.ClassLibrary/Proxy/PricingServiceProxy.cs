using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto; // Import the existing DTOs namespace
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class PricingServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IPricingService
{
    private const string BaseUrl = "api/pricing";

    public async Task<float> CalculateBasePriceAsync(Flight flight)
    {
        return await PostForResultAsync<Flight, float>($"{BaseUrl}/calculate-base-price", flight);
    }

    public async Task<float> CalculateTotalPriceAsync(FlightTicket flightTicket)
    {
        return await PostForResultAsync<FlightTicket, float>($"{BaseUrl}/calculate-total-price", flightTicket);
    }

    public async Task<PriceBreakdown> CalculatePriceBreakdownAsync(Flight flight, Customer user, List<FlightTicket> tickets)
    {
        var payload = new CalculatePriceBreakdownRequestDTO
        {
            FlightData = new CalculateBasePriceRequestDTO
            {
                DepartureTime = flight?.Route?.DepartureTime != null
                                ? new DateTime(1, 1, 1, flight.Route.DepartureTime.Hour, flight.Route.DepartureTime.Minute, flight.Route.DepartureTime.Second)
                                : DateTime.MinValue,
                ArrivalTime = flight?.Route?.ArrivalTime != null
                                ? new DateTime(1, 1, 1, flight.Route.ArrivalTime.Hour, flight.Route.ArrivalTime.Minute, flight.Route.ArrivalTime.Second)
                                : DateTime.MinValue
            },
            FlightDiscountPercentage = user?.Membership?.FlightDiscountPercentage ?? 0f,
            AddonDiscounts = user?.Membership?.AddonDiscounts?.Select(d => new PricingAddOnDiscountDTO
            {
                AddOnId = d.AddOn?.Id ?? 0,
                DiscountPercentage = d.DiscountPercentage
            }).ToList() ?? new List<PricingAddOnDiscountDTO>(),
            TicketsAddOns = tickets?.Select(t => t.SelectedAddOns?.Select(a => new PricingAddOnDTO
            {
                Id = a.Id,
                BasePrice = a.BasePrice
            }).ToList() ?? new List<PricingAddOnDTO>()).ToList() ?? new List<List<PricingAddOnDTO>>()
        };

        return await PostForResultAsync<CalculatePriceBreakdownRequestDTO, PriceBreakdown>($"{BaseUrl}/calculate-breakdown", payload);
    }
}