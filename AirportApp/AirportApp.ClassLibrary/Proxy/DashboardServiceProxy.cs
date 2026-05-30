using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class DashboardServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IDashBoardService
{
    private const string BaseUrl = "api/dashboard";

    public async Task<IEnumerable<FlightTicket>> GetUserTicketsAsync(int userId, string ticketFilter)
    {
        return await GetListAsync<FlightTicket>($"{BaseUrl}/user/{userId}/tickets?filter={Uri.EscapeDataString(ticketFilter)}");
    }

    public async Task<IEnumerable<FlightTicket>> GetTicketsByUserIdAsync(int userId)
    {
        return await GetListAsync<FlightTicket>($"{BaseUrl}/user/{userId}/all-tickets");
    }

    public string GenerateTicketPdf(FlightTicket ticket)
    {
        throw new NotSupportedException("GenerateTicketPdf is not supported on the proxy.");
    }

    public async Task AddTicketAsync(FlightTicket ticket)
    {
        await PostAsync($"{BaseUrl}/ticket", ticket);
    }

    public async Task UpdateTicketStatusAsync(int ticketId, string status)
    {
        await PutAsync($"{BaseUrl}/ticket/{ticketId}/status", status);
    }

    public async Task AddTicketAddOnsAsync(int ticketId, IEnumerable<int> addOnIds)
    {
        await PostAsync($"{BaseUrl}/ticket/{ticketId}/addons", addOnIds);
    }

    public async Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId)
    {
        return await GetListAsync<string>($"{BaseUrl}/flight/{flightId}/occupied-seats");
    }

    public async Task<bool> IsSeatAvailableAsync(int flightId, string seat)
    {
        return await GetRequiredAsync<bool>($"{BaseUrl}/flight/{flightId}/seat/{seat}/available");
    }

    public async Task<bool> SaveTicketsWithAddOnsAsync(List<FlightTicket> tickets, List<List<int>> addOnIds)
    {
        var request = new { Tickets = tickets, AddOnIds = addOnIds };
        return await PostForResultAsync<object, bool>($"{BaseUrl}/tickets-addons", request);
    }
}
