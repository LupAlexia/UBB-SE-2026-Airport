using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class EfAirportRepository(AppDbContext databaseContext) : IAirportRepository
{
    public async Task<IEnumerable<Airport>> GetAsync()
    {
        return await databaseContext.Airports
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Airport?> GetByIdAsync(int airportId)
    {
        return await databaseContext.Airports.FindAsync(airportId);
    }

    public async Task<int> AddAsync(Airport airport)
    {
        if (airport is null)
        {
            throw new ArgumentNullException(nameof(airport));
        }

        airport.Id = 0;
        databaseContext.Airports.Add(airport);
        await databaseContext.SaveChangesAsync();

        return airport.Id;
    }

    public async Task UpdateAsync(Airport airport)
    {
        if (airport is null)
        {
            throw new ArgumentNullException(nameof(airport));
        }

        var existingAirport = await databaseContext.Airports.FindAsync(airport.Id);
        if (existingAirport is null)
        {
            return;
        }

        existingAirport.AirportCode = airport.AirportCode;
        existingAirport.Name = airport.Name;
        existingAirport.City = airport.City;

        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int airportId)
    {
        var airportToRemove = await GetByIdAsync(airportId);
        if (airportToRemove is null)
        {
            return;
        }

        databaseContext.Airports.Remove(airportToRemove);
        await databaseContext.SaveChangesAsync();
    }
}