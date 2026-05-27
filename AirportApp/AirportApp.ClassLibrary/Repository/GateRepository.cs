using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class GateRepository(AppDbContext databaseContext) : IGateRepository
{
    public async Task<IEnumerable<Gate>> GetAsync()
    {
        return await databaseContext.Gates
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Gate?> GetByIdAsync(int gateId)
    {
        return await databaseContext.Gates.FindAsync(gateId);
    }

    public async Task<int> AddAsync(Gate gate)
    {
        if (gate is null)
        {
            throw new ArgumentNullException(nameof(gate));
        }

        gate.Id = 0;
        databaseContext.Gates.Add(gate);
        await databaseContext.SaveChangesAsync();

        return gate.Id;
    }

    public async Task UpdateAsync(Gate gate)
    {
        if (gate is null)
        {
            throw new ArgumentNullException(nameof(gate));
        }

        var existingGate = await databaseContext.Gates.FindAsync(gate.Id);
        if (existingGate is null)
        {
            return;
        }

        existingGate.GateName = gate.GateName;

        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int gateId)
    {
        var associatedFlights = await databaseContext.Flights
            .Where(flight => flight.Gate.Id == gateId)
            .ToListAsync();

        if (associatedFlights.Count > 0)
        {
            databaseContext.Flights.RemoveRange(associatedFlights);
        }

        var gateToRemove = await databaseContext.Gates.FindAsync(gateId);
        if (gateToRemove is null)
        {
            return;
        }

        databaseContext.Gates.Remove(gateToRemove);

        try
        {
            await databaseContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        {
            System.Diagnostics.Debug.WriteLine($"Database Update Exception: {exception.InnerException?.Message}");
            throw;
        }
    }
}