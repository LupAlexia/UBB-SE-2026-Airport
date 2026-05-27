using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class RunwayRepository(AppDbContext databaseContext) : IRunwayRepository
{
    private const string RunwayIdShadowProperty = "RunwayId";

    public async Task<IEnumerable<Runway>> GetAsync()
    {
        return await databaseContext.Runways
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Runway?> GetByIdAsync(int runwayId)
    {
        return await databaseContext.Runways.FindAsync(runwayId);
    }

    public async Task<int> AddAsync(Runway runway)
    {
        if (runway is null)
        {
            throw new ArgumentNullException(nameof(runway));
        }

        runway.Id = 0;
        databaseContext.Runways.Add(runway);
        await databaseContext.SaveChangesAsync();

        return runway.Id;
    }

    public async Task UpdateAsync(Runway runway)
    {
        if (runway is null)
        {
            throw new ArgumentNullException(nameof(runway));
        }

        var existingRunway = await databaseContext.Runways.FindAsync(runway.Id);
        if (existingRunway is null)
        {
            return;
        }

        existingRunway.Name = runway.Name;
        existingRunway.HandleTime = runway.HandleTime;

        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int runwayId)
    {
        var associatedFlights = await databaseContext.Flights
            .Where(flight => EF.Property<int>(flight, RunwayIdShadowProperty) == runwayId)
            .ToListAsync();

        if (associatedFlights.Count > 0)
        {
            databaseContext.Flights.RemoveRange(associatedFlights);
        }

        var runwayToDelete = await databaseContext.Runways.FindAsync(runwayId);
        if (runwayToDelete is null)
        {
            return;
        }

        databaseContext.Runways.Remove(runwayToDelete);

        try
        {
            await databaseContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            foreach (var flightInstance in associatedFlights)
            {
                databaseContext.Entry(flightInstance).State = EntityState.Unchanged;
            }

            databaseContext.Entry(runwayToDelete).State = EntityState.Unchanged;
            throw;
        }
    }
}