using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class FlightTicketRepository(AppDbContext databaseContext) : IFlightTicketRepository
{
    private const string CancelledStatusLower = "canceled";
    private const string CancelledStatusUpper = "Cancelled";
    private const string UserIdProperty = "UserId";
    private const string FlightIdProperty = "FlightId";

    public async Task<IEnumerable<FlightTicket>> GetByUserIdAsync(int userId)
    {
        return await databaseContext.FlightTickets
            .Where(ticket => EF.Property<int>(ticket, UserIdProperty) == userId)
            .Include(ticket => ticket.User)
            .Include(ticket => ticket.Flight)
                .ThenInclude(flight => flight.Route)
                    .ThenInclude(route => route.Airport)
            .Include(ticket => ticket.Flight)
                .ThenInclude(flight => flight.Route)
                    .ThenInclude(route => route.Company)
            .Include(ticket => ticket.Flight)
                .ThenInclude(flight => flight.Gate)
            .Include(ticket => ticket.SelectedAddOns)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> AddAsync(FlightTicket flightTicket)
    {
        if (flightTicket is null)
        {
            throw new ArgumentNullException(nameof(flightTicket));
        }

        if (flightTicket.User is null || flightTicket.User.Id <= 0)
        {
            throw new ArgumentException("Ticket must contain a valid user.", nameof(flightTicket));
        }

        if (flightTicket.Flight is null || flightTicket.Flight.Id <= 0)
        {
            throw new ArgumentException("Ticket must contain a valid flight.", nameof(flightTicket));
        }

        int userId = flightTicket.User.Id;
        int flightId = flightTicket.Flight.Id;

        flightTicket.User = null!;
        flightTicket.Flight = null!;

        databaseContext.FlightTickets.Add(flightTicket);

        databaseContext.Entry(flightTicket).Property(UserIdProperty).CurrentValue = userId;
        databaseContext.Entry(flightTicket).Property(FlightIdProperty).CurrentValue = flightId;

        await databaseContext.SaveChangesAsync();
        return flightTicket.Id;
    }

    public async Task UpdateStatusAsync(int flightTicketId, string status)
    {
        var ticket = await databaseContext.FlightTickets.FindAsync(flightTicketId);
        if (ticket is null)
        {
            return;
        }

        ticket.Status = status;
        await databaseContext.SaveChangesAsync();
    }

    public async Task AddAddOnsAsync(int flightTicketId, IEnumerable<int> addOnIds)
    {
        if (addOnIds is null || !addOnIds.Any())
        {
            return;
        }

        var ticket = await databaseContext.FlightTickets
            .Include(flightTicket => flightTicket.SelectedAddOns)
            .FirstOrDefaultAsync(flightTicket => flightTicket.Id == flightTicketId);

        if (ticket is null)
        {
            return;
        }

        foreach (int addOnId in addOnIds)
        {
            var addOn = await databaseContext.AddOns.FindAsync(addOnId);
            if (addOn is not null && !ticket.SelectedAddOns.Contains(addOn))
            {
                ticket.SelectedAddOns.Add(addOn);
            }
        }

        await databaseContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId)
    {
        return await databaseContext.FlightTickets
            .Where(ticket => EF.Property<int>(ticket, FlightIdProperty) == flightId
                             && ticket.Status != CancelledStatusUpper
                             && ticket.Status != CancelledStatusLower
                             && ticket.Seat != null)
            .Select(ticket => ticket.Seat)
            .ToListAsync();
    }

    public async Task<bool> IsSeatAvailableAsync(int flightId, string seat)
    {
        bool isOccupied = await databaseContext.FlightTickets
            .AnyAsync(ticket => EF.Property<int>(ticket, FlightIdProperty) == flightId
                                && ticket.Seat == seat
                                && ticket.Status != CancelledStatusUpper
                                && ticket.Status != CancelledStatusLower);

        return !isOccupied;
    }

    public async Task<bool> SaveBatchWithAddOnsAsync(List<FlightTicket> tickets, List<List<int>> addOnIds)
    {
        try
        {
            for (int index = 0; index < tickets.Count; index++)
            {
                var ticket = tickets[index];
                var currentTicketAddOnIds = addOnIds != null && index < addOnIds.Count ? addOnIds[index] : [];

                await PrepareTicketForStorageAsync(ticket, currentTicketAddOnIds);
                databaseContext.FlightTickets.Add(ticket);
            }

            await databaseContext.SaveChangesAsync();
            return true;
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Batch save failed: {exception.Message}");
            return false;
        }
    }

    private async Task PrepareTicketForStorageAsync(FlightTicket ticket, List<int> addOnIds)
    {
        int userId = ticket.User?.Id ?? 0;
        int flightId = ticket.Flight?.Id ?? 0;

        ticket.User = null!;
        ticket.Flight = null!;

        ticket.SelectedAddOns = [];
        foreach (int id in addOnIds)
        {
            var existingAddOn = await databaseContext.AddOns.FindAsync(id);
            if (existingAddOn is not null)
            {
                ticket.SelectedAddOns.Add(existingAddOn);
            }
        }

        var entry = databaseContext.Entry(ticket);
        if (userId > 0)
        {
            entry.Property(UserIdProperty).CurrentValue = userId;
        }

        if (flightId > 0)
        {
            entry.Property(FlightIdProperty).CurrentValue = flightId;
        }
    }
}