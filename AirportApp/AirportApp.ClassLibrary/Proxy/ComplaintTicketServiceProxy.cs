using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class ComplaintTicketServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IComplaintTicketService
{
    private const string BaseUrl = "api/ticket";

    public async Task CreateTicketAsync(int ticketId, User ticketCreator, ComplaintTicketStatusEnum initialStatus,
        ComplaintTicketCategory category, ComplaintTicketSubcategory subcategory,
        string subject, string description, DateTime creationTimestamp,
        ComplaintTicketUrgencyLevelEnum? initialUrgencyLevel = null)
    {
        var ticket = new ComplaintTicket
        {
            Id = ticketId,
            Creator = ticketCreator,
            CurrentStatus = initialStatus,
            Category = category,
            Subcategory = subcategory,
            Subject = subject,
            Description = description,
            CreationTimestamp = creationTimestamp,
            UrgencyLevel = initialUrgencyLevel ?? ComplaintTicketUrgencyLevelEnum.LOW
        };
        await AddTicketAsync(ticket);
    }

    public async Task AddTicketAsync(ComplaintTicket ticketEntity)
    {
        await PostAsync(BaseUrl, ticketEntity);
    }

    public async Task DeleteTicketByIdAsync(int ticketId)
    {
        await DeleteAsync($"{BaseUrl}/{ticketId}");
    }

    public async Task<ComplaintTicket> GetTicketByIdAsync(int ticketId)
    {
        return await GetRequiredAsync<ComplaintTicket>($"{BaseUrl}/{ticketId}");
    }

    public async Task<IEnumerable<ComplaintTicket>> GetAllTicketsAsync()
    {
        return await GetListAsync<ComplaintTicket>(BaseUrl);
    }

    public async Task UpdateTicketByIdAsync(int id, ComplaintTicket ticket)
    {
        await PutAsync($"{BaseUrl}/{id}", ticket);
    }

    public async Task UpdateUrgencyLevelAsync(int ticketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
    {
        await PutAsync($"{BaseUrl}/{ticketId}/urgency", newUrgencyLevel);
    }

    public async Task UpdateStatusAsync(int ticketId, ComplaintTicketStatusEnum newStatus)
    {
        await PutAsync($"{BaseUrl}/{ticketId}/status", newStatus);
    }

    public async Task<IEnumerable<TicketDTO>> FilterTicketsByStatusAsync(IEnumerable<TicketDTO> tickets, TicketFilterStatusEnum filter)
    {
        return await PostForResultAsync<IEnumerable<TicketDTO>, IEnumerable<TicketDTO>>($"{BaseUrl}/filter?filter={filter}", tickets);
    }
}
