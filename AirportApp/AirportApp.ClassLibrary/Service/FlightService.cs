using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class FlightService(IFlightRepository flightRepository) : IFlightService
{
    public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
    {
        return await flightRepository.GetAsync();
    }

    public async Task<Flight?> GetFlightByIdAsync(int flightId)
    {
        return await flightRepository.GetByIdAsync(flightId);
    }

    public async Task AddFlightAsync(Flight flight)
    {
        if (string.IsNullOrWhiteSpace(flight.FlightNumber))
            throw new ArgumentException("Flight number cannot be empty.");
        if (flight.Route is null || flight.Route.Id <= 0)
            throw new ArgumentException("A valid route must be assigned to the flight.");
        await flightRepository.AddAsync(flight);
    }

    public async Task UpdateFlightAsync(Flight flight)
    {
        var existing = await flightRepository.GetByIdAsync(flight.Id);
        if (existing is null)
            throw new InvalidOperationException($"Flight with ID {flight.Id} not found.");

        if (flight.FlightNumber is not null)
            existing.FlightNumber = flight.FlightNumber;
        if (flight.Gate is not null)
            existing.Gate = flight.Gate;
        if (flight.Runway is not null)
            existing.Runway = flight.Runway;
        if (flight.Route is not null)
            existing.Route = flight.Route;
        if (flight.Date != default)
            existing.Date = flight.Date;

        await flightRepository.UpdateAsync(existing);
    }

    public async Task DeleteFlightAsync(int flightId)
    {
        await flightRepository.DeleteAsync(flightId);
    }

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(string location, string routeType, DateTime? date)
    {
        return await flightRepository.SearchFlightsAsync(location, routeType, date);
    }

    public async Task<int> GetOccupiedSeatCountAsync(int flightId)
    {
        return await flightRepository.GetOccupiedSeatCountAsync(flightId);
    }
    public async Task<IEnumerable<Flight>> GetFlightsByRouteIdAsync(int routeId)
    {
        return await flightRepository.GetByRouteIdAsync(routeId);
    }
}
