using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class ComplaintTicketRepository(AppDbContext databaseContext) : IComplaintTicketRepository
{
    public async Task<IEnumerable<ComplaintTicket>> GetAsync()
    {
        return await databaseContext.Tickets
            .Include(ticket => ticket.Creator)
            .Include(ticket => ticket.Category)
            .Include(ticket => ticket.Subcategory)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ComplaintTicket?> GetByIdAsync(int complaintTicketId)
    {
        return await databaseContext.Tickets
            .Include(ticket => ticket.Creator)
            .Include(ticket => ticket.Category)
            .Include(ticket => ticket.Subcategory)
            .FirstOrDefaultAsync(ticket => ticket.Id == complaintTicketId);
    }

    public async Task<int> AddAsync(ComplaintTicket complaintTicket)
    {
        if (complaintTicket is null)
        {
            throw new ArgumentNullException(nameof(complaintTicket));
        }

        complaintTicket.Id = 0;
        databaseContext.Tickets.Add(complaintTicket);
        await databaseContext.SaveChangesAsync();

        return complaintTicket.Id;
    }

    public async Task UpdateAsync(ComplaintTicket complaintTicket)
    {
        if (complaintTicket is null)
        {
            throw new ArgumentNullException(nameof(complaintTicket));
        }

        var existingTicket = await databaseContext.Tickets.FindAsync(complaintTicket.Id);
        if (existingTicket is null)
        {
            return;
        }

        existingTicket.Subject = complaintTicket.Subject;
        existingTicket.Description = complaintTicket.Description;
        existingTicket.CurrentStatus = complaintTicket.CurrentStatus;
        existingTicket.UrgencyLevel = complaintTicket.UrgencyLevel;

        await databaseContext.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int complaintTicketId, ComplaintTicketStatusEnum newStatus)
    {
        var ticket = await databaseContext.Tickets.FindAsync(complaintTicketId);

        if (ticket is null)
        {
            throw new KeyNotFoundException($"Ticket {complaintTicketId} not found.");
        }

        ticket.CurrentStatus = newStatus;
        await databaseContext.SaveChangesAsync();
    }

    public async Task UpdateUrgencyAsync(int complaintTicketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
    {
        var ticket = await databaseContext.Tickets.FindAsync(complaintTicketId);

        if (ticket is null)
        {
            throw new KeyNotFoundException($"Ticket {complaintTicketId} not found.");
        }

        ticket.UrgencyLevel = newUrgencyLevel;
        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int complaintTicketId)
    {
        var ticket = await databaseContext.Tickets.FindAsync(complaintTicketId);
        if (ticket is null)
        {
            return;
        }

        databaseContext.Tickets.Remove(ticket);
        await databaseContext.SaveChangesAsync();
    }
}