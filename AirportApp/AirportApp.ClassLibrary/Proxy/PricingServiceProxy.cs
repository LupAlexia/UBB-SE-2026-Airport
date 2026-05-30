using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class PricingServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IPricingService
{
    private const string BaseUrl = "api/pricing";

    public async Task<float> CalculateBasePriceAsync(Flight flight)
    {
        return await PostForResultAsync<Flight, float>($"{BaseUrl}/base-price", flight);
    }

    public async Task<float> CalculateTotalPriceAsync(FlightTicket flightTicket)
    {
        return await PostForResultAsync<FlightTicket, float>($"{BaseUrl}/total-price", flightTicket);
    }

    public async Task<PriceBreakdown> CalculatePriceBreakdownAsync(Flight flight, Customer user, List<FlightTicket> tickets)
    {
        var payload = new { Flight = flight, User = user, Tickets = tickets };
        return await PostForResultAsync<object, PriceBreakdown>($"{BaseUrl}/breakdown", payload);
    }
}
