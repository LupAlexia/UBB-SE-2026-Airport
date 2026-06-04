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
        var ticket = new CreateTicketDTO(
            ticketCreator.Id,
            category.Id,
            subcategory.Id,
            subject,
            description,
            creationTimestamp,
            initialStatus,
            initialUrgencyLevel ?? category.CategoryUrgencyLevel);

        await PostAsync(BaseUrl, ticket);
    }

    public async Task AddTicketAsync(ComplaintTicket ticketEntity)
    {
        var ticket = new CreateTicketDTO(
            ticketEntity.Creator.Id,
            ticketEntity.Category.Id,
            ticketEntity.Subcategory.Id,
            ticketEntity.Subject,
            ticketEntity.Description,
            ticketEntity.CreationTimestamp,
            ticketEntity.CurrentStatus,
            ticketEntity.UrgencyLevel);

        await PostAsync(BaseUrl, ticket);
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
        var requestPayload = new { UrgencyLevel = newUrgencyLevel };
        await PutAsync($"{BaseUrl}/{ticketId}/urgency", requestPayload);
    }

    public async Task UpdateStatusAsync(int ticketId, ComplaintTicketStatusEnum newStatus)
    {
        var requestPayload = new { CurrentStatus = newStatus };
        await PutAsync($"{BaseUrl}/{ticketId}/status", requestPayload);
    }

    public async Task<IEnumerable<TicketDTO>> FilterTicketsByStatusAsync(IEnumerable<TicketDTO> tickets, TicketFilterStatusEnum filter)
    {
        return await PostForResultAsync<IEnumerable<TicketDTO>, IEnumerable<TicketDTO>>($"{BaseUrl}/filter?filter={filter}", tickets);
    }
}
