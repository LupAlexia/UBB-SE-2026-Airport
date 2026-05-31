using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class DashboardServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IDashboardService
{
    private const string BaseUrl = "api/flightticket";

    public async Task<IEnumerable<FlightTicket>> GetUserTicketsAsync(int userId, string ticketFilter)
    {
        var dtos = await GetListAsync<FlightTicketDTO>($"{BaseUrl}/user/{userId}/filter?filter={Uri.EscapeDataString(ticketFilter)}");
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<IEnumerable<FlightTicket>> GetTicketsByUserIdAsync(int userId)
    {
        var dtos = await GetListAsync<FlightTicketDTO>($"{BaseUrl}/user/{userId}");
        return dtos.Select(MapToEntity).ToList();
    }

    public string GenerateTicketPdf(FlightTicket ticket)
    {
        throw new NotSupportedException("GenerateTicketPdf is not supported on the proxy.");
    }

    public async Task AddTicketAsync(FlightTicket ticket)
    {
        await PostAsync(BaseUrl, MapToDto(ticket));
    }

    public async Task UpdateTicketStatusAsync(int ticketId, string status)
    {
        await PutAsync($"{BaseUrl}/{ticketId}/status", status);
    }

    public async Task AddTicketAddOnsAsync(int ticketId, IEnumerable<int> addOnIds)
    {
        await PostAsync($"{BaseUrl}/{ticketId}/addons", addOnIds);
    }

    public async Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId)
    {
        return await GetListAsync<string>($"{BaseUrl}/flight/{flightId}/occupied-seats");
    }

    public async Task<bool> IsSeatAvailableAsync(int flightId, string seat)
    {
        return await GetRequiredAsync<bool>($"{BaseUrl}/flight/{flightId}/seat-available/{seat}");
    }

    public async Task<bool> SaveTicketsWithAddOnsAsync(List<FlightTicket> tickets, List<List<int>> addOnIds)
    {
        var request = new SaveTicketsRequestDTO
        {
            Tickets = tickets.Select(MapToDto).ToList(),
            AddOnIds = addOnIds
        };
        return await PostForResultAsync<SaveTicketsRequestDTO, bool>($"{BaseUrl}/batch", request);
    }

    private static FlightTicket MapToEntity(FlightTicketDTO dto)
    {
        var ticket = new FlightTicket
        {
            Id = dto.id,
            User = new Customer { Id = dto.userId },
            Flight = dto.flight != null ? FlightServiceProxy.MapToEntity(dto.flight) : new Flight { Id = dto.flightId },
            Seat = dto.seat,
            Price = dto.price,
            Status = dto.status,
            PassengerFirstName = dto.passengerFirstName,
            PassengerLastName = dto.passengerLastName,
            PassengerEmail = dto.passengerEmail,
            PassengerPhone = dto.passengerPhone,
            SelectedAddOns = dto.selectedAddOns?.Select(addOn => new AddOn { Id = addOn.id, Name = addOn.name, BasePrice = addOn.basePrice }).ToList()
                ?? new List<AddOn>()
        };

        return ticket;
    }

    private static FlightTicketDTO MapToDto(FlightTicket ticket)
    {
        var addOns = ticket.SelectedAddOns?.Select(addOn => new AddOnDTO(addOn.Id, addOn.Name, addOn.BasePrice)).ToList();
        var flightDto = ticket.Flight != null ? FlightServiceProxy.MapToDto(ticket.Flight) : null;

        return new FlightTicketDTO(
            ticket.Id,
            ticket.User?.Id ?? 0,
            ticket.Flight?.Id ?? 0,
            ticket.Seat,
            ticket.Price,
            ticket.Status,
            ticket.PassengerFirstName,
            ticket.PassengerLastName,
            ticket.PassengerEmail,
            ticket.PassengerPhone,
            addOns,
            flightDto);
    }
}
