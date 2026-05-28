using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class RouteRepository(AppDbContext databaseContext) : IRouteRepository
{
    public async Task<IEnumerable<Route>> GetAsync()
    {
        return await databaseContext.Routes
            .Include(route => route.Company)
            .Include(route => route.Airport)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Route?> GetByIdAsync(int routeId)
    {
        return await databaseContext.Routes
            .Include(route => route.Company)
            .Include(route => route.Airport)
            .FirstOrDefaultAsync(route => route.Id == routeId);
    }

    public async Task<int> AddAsync(Route route)
    {
        if (route is null)
        {
            throw new ArgumentNullException(nameof(route));
        }

        route.Company = await databaseContext.Companies.FindAsync(route.Company.Id)
            ?? throw new InvalidOperationException($"Company {route.Company.Id} not found.");

        route.Airport = await databaseContext.Airports.FindAsync(route.Airport.Id)
            ?? throw new InvalidOperationException($"Airport {route.Airport.Id} not found.");

        route.Id = 0;
        databaseContext.Routes.Add(route);
        await databaseContext.SaveChangesAsync();

        return route.Id;
    }

    public async Task UpdateAsync(Route route)
    {
        if (route is null)
        {
            throw new ArgumentNullException(nameof(route));
        }

        var existingRoute = await databaseContext.Routes.FindAsync(route.Id);
        if (existingRoute is null)
        {
            return;
        }

        var company = await databaseContext.Companies.FindAsync(route.Company.Id)
            ?? throw new InvalidOperationException($"Company {route.Company.Id} not found.");

        var airport = await databaseContext.Airports.FindAsync(route.Airport.Id)
            ?? throw new InvalidOperationException($"Airport {route.Airport.Id} not found.");

        existingRoute.RouteType = route.RouteType;
        existingRoute.RecurrenceInterval = route.RecurrenceInterval;
        existingRoute.StartDate = route.StartDate;
        existingRoute.EndDate = route.EndDate;
        existingRoute.DepartureTime = route.DepartureTime;
        existingRoute.ArrivalTime = route.ArrivalTime;
        existingRoute.Capacity = route.Capacity;
        existingRoute.Company = company;
        existingRoute.Airport = airport;

        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int routeId)
    {
        var routeToRemove = await databaseContext.Routes.FindAsync(routeId);

        if (routeToRemove is null)
        {
            return;
        }

        databaseContext.Routes.Remove(routeToRemove);
        await databaseContext.SaveChangesAsync();
    }
}