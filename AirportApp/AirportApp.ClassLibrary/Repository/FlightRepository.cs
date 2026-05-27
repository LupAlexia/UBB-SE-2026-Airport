using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class FlightRepository(AppDbContext databaseContext) : IFlightRepository
{
    private const string CancelledStatusLower = "canceled";
    private const string CancelledStatusUpper = "Cancelled";
    private IQueryable<Flight> CompleteFlightGraph => databaseContext.Flights
        .Include(flight => flight.Route)
            .ThenInclude(route => route.Company)
        .Include(flight => flight.Route)
            .ThenInclude(route => route.Airport)
        .Include(flight => flight.Runway)
        .Include(flight => flight.Gate);

    public async Task<IEnumerable<Flight>> GetAsync()
    {
        return await CompleteFlightGraph
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Flight?> GetByIdAsync(int flightId)
    {
        return await CompleteFlightGraph
            .FirstOrDefaultAsync(flight => flight.Id == flightId);
    }

    public async Task<IEnumerable<Flight>> GetByRouteIdAsync(int routeId)
    {
        return await CompleteFlightGraph
            .Where(flight => flight.Route.Id == routeId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Flight>> GetByRunwayIdAsync(int runwayId)
    {
        return await CompleteFlightGraph
            .Where(flight => flight.Runway.Id == runwayId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Flight>> GetByGateIdAsync(int gateId)
    {
        return await CompleteFlightGraph
            .Where(flight => flight.Gate.Id == gateId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Flight>> GetByAirportIdAsync(int airportId)
    {
        return await CompleteFlightGraph
            .Where(flight => flight.Route.Airport.Id == airportId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> AddAsync(Flight flight)
    {
        if (flight is null)
        {
            throw new ArgumentNullException(nameof(flight));
        }

        flight.Route = await databaseContext.Routes.FindAsync(flight.Route.Id)
            ?? throw new InvalidOperationException($"Route {flight.Route.Id} not found.");

        flight.Runway = await databaseContext.Runways.FindAsync(flight.Runway.Id)
            ?? throw new InvalidOperationException($"Runway {flight.Runway.Id} not found.");

        flight.Gate = await databaseContext.Gates.FindAsync(flight.Gate.Id)
            ?? throw new InvalidOperationException($"Gate {flight.Gate.Id} not found.");

        flight.Id = 0;
        databaseContext.Flights.Add(flight);
        await databaseContext.SaveChangesAsync();

        return flight.Id;
    }

    public async Task UpdateAsync(Flight flight)
    {
        if (flight is null)
        {
            throw new ArgumentNullException(nameof(flight));
        }

        var existingFlight = await databaseContext.Flights.FindAsync(flight.Id);
        if (existingFlight is null)
        {
            return;
        }

        var route = await databaseContext.Routes.FindAsync(flight.Route.Id)
            ?? throw new InvalidOperationException($"Route {flight.Route.Id} not found.");

        var runway = await databaseContext.Runways.FindAsync(flight.Runway.Id)
            ?? throw new InvalidOperationException($"Runway {flight.Runway.Id} not found.");

        var gate = await databaseContext.Gates.FindAsync(flight.Gate.Id)
            ?? throw new InvalidOperationException($"Gate {flight.Gate.Id} not found.");

        existingFlight.Date = flight.Date;
        existingFlight.FlightNumber = flight.FlightNumber;
        existingFlight.Route = route;
        existingFlight.Runway = runway;
        existingFlight.Gate = gate;

        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int flightId)
    {
        var flightToRemove = await databaseContext.Flights.FindAsync(flightId);
        if (flightToRemove is null)
        {
            return;
        }

        databaseContext.Flights.Remove(flightToRemove);
        await databaseContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(string location, string routeType, DateTime? date)
    {
        var query = CompleteFlightGraph;

        if (date.HasValue)
        {
            query = query.Where(flight => flight.Date.Date == date.Value.Date);
        }

        query = query.Where(flight => flight.Route.RouteType == routeType);

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(flight =>
                flight.Route.Airport.City == location ||
                flight.Route.Airport.AirportCode == location);
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<int> GetOccupiedSeatCountAsync(int flightId)
    {
        return await databaseContext.FlightTickets
            .Where(ticket => ticket.Status != CancelledStatusLower && ticket.Status != CancelledStatusUpper)
            .Where(ticket => ticket.Flight.Id == flightId)
            .CountAsync();
    }
}