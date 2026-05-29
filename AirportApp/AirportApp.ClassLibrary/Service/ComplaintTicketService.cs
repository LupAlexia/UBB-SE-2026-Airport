using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ComplaintTicketService(IComplaintTicketRepository ticketRepository) : IComplaintTicketService
{
    public async Task CreateTicketAsync(int ticketId, User ticketCreator, ComplaintTicketStatusEnum initialStatus,
        ComplaintTicketCategory category, ComplaintTicketSubcategory subcategory,
        string subject, string description, DateTime creationTimestamp,
        ComplaintTicketUrgencyLevelEnum? initialUrgencyLevel = null)
    {
        ComplaintTicket newTicket = new ComplaintTicket(ticketId, ticketCreator, initialStatus, category, subcategory, subject, description, creationTimestamp, initialUrgencyLevel);
        ValidateTicket(newTicket);
        await AddTicketAsync(newTicket);
    }

    public async Task AddTicketAsync(ComplaintTicket ticketEntity)
    {
        await ticketRepository.AddAsync(ticketEntity);
    }

    public async Task DeleteTicketByIdAsync(int ticketId)
    {
        await ticketRepository.DeleteAsync(ticketId);
    }

    public async Task<ComplaintTicket> GetTicketByIdAsync(int ticketId)
    {
        return await ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException($"Ticket {ticketId} not found.");
    }

    public async Task<IEnumerable<ComplaintTicket>> GetAllTicketsAsync()
    {
        return await ticketRepository.GetAsync();
    }

    public async Task UpdateTicketByIdAsync(int identificationNumber, ComplaintTicket ticket)
    {
        await ticketRepository.UpdateAsync(ticket);
    }

    private void ValidateTicket(ComplaintTicket ticket)
    {
        if (ticket == null)
            throw new ArgumentNullException("The newTicket does not have any data.");
        if (ticket.Creator == null)
            throw new ArgumentNullException("The ticketCreator does not have any data.");
        if (ticket.Category == null)
            throw new ArgumentNullException("Null Category.");
        if (ticket.Subcategory == null)
            throw new ArgumentNullException("Null Subcategory.");
        if (ticket.Subcategory.ParentCategory.Id != ticket.Category.Id)
            throw new ArgumentException($"The subcategory '{ticket.Subcategory.SubcategoryName}' does not belong to the category '{ticket.Category.CategoryName}'");
        if (string.IsNullOrWhiteSpace(ticket.Subject))
            throw new ArgumentNullException("The Subject is empty.");
        if (string.IsNullOrWhiteSpace(ticket.Description))
            throw new ArgumentNullException("The Description is empty.");
    }

    public async Task UpdateUrgencyLevelAsync(int ticketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
    {
        await ticketRepository.UpdateUrgencyAsync(ticketId, newUrgencyLevel);
    }

    public async Task UpdateStatusAsync(int ticketId, ComplaintTicketStatusEnum newStatus)
    {
        await ticketRepository.UpdateStatusAsync(ticketId, newStatus);
    }

    public Task<IEnumerable<TicketDTO>> FilterTicketsByStatusAsync(IEnumerable<TicketDTO> tickets, TicketFilterStatusEnum filter)
    {
        IEnumerable<TicketDTO> filteredTickets;
        switch (filter)
        {
            case TicketFilterStatusEnum.OPEN:
                filteredTickets = tickets.Where(t => t.currentStatus == ComplaintTicketStatusEnum.OPEN);
                break;
            case TicketFilterStatusEnum.IN_PROGRESS:
                filteredTickets = tickets.Where(t => t.currentStatus == ComplaintTicketStatusEnum.IN_PROGRESS);
                break;
            case TicketFilterStatusEnum.RESOLVED:
                filteredTickets = tickets.Where(t => t.currentStatus == ComplaintTicketStatusEnum.RESOLVED);
                break;
            default:
                filteredTickets = tickets;
                break;
        }
        return Task.FromResult(filteredTickets);
    }
}
