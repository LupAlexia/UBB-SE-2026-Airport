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
        if (flightId <= 0)
        {
            return null;
        }

        return await flightRepository.GetByIdAsync(flightId);
    }

    public async Task<IEnumerable<Flight>> GetFlightsByRouteIdAsync(int routeId)
    {
        if (routeId <= 0)
        {
            return new List<Flight>();
        }

        return await flightRepository.GetByRouteIdAsync(routeId);
    }

    public async Task<int> AddFlightAsync(string flightNumber, int routeId, DateTime date, int runwayId, int gateId)
    {
        if (string.IsNullOrWhiteSpace(flightNumber))
        {
            throw new ArgumentException("The flight number cannot be empty.");
        }

        if (routeId <= 0)
        {
            throw new ArgumentException("A valid route Id is required.");
        }

        Flight newFlight = new Flight
        {
            FlightNumber = flightNumber,
            Route = new Route { Id = routeId },
            Date = date,
            Runway = new Runway { Id = runwayId },
            Gate = new Gate { Id = gateId }
        };

        return await flightRepository.AddAsync(newFlight);
    }

    public async Task UpdateFlightAsync(
        int flightId,
        DateTime? date = null,
        string? flightNumber = null,
        int? runwayId = null,
        int? gateId = null)
    {
        Flight? existingFlight = await flightRepository.GetByIdAsync(flightId);

        if (existingFlight == null)
        {
            throw new InvalidOperationException($"Flight with Id {flightId} does not exist in the system.");
        }

        if (date.HasValue)
        {
            existingFlight.Date = date.Value;
        }

        if (flightNumber != null)
        {
            existingFlight.FlightNumber = flightNumber;
        }

        if (runwayId.HasValue)
        {
            existingFlight.Runway = new Runway { Id = runwayId.Value };
        }

        if (gateId.HasValue)
        {
            existingFlight.Gate = new Gate { Id = gateId.Value };
        }

        await flightRepository.UpdateAsync(existingFlight);
    }

    public async Task DeleteFlightAsync(int flightId)
    {
        if (flightId <= 0)
        {
            throw new ArgumentException("The provided flight Id is invalid.");
        }

        if (await flightRepository.GetByIdAsync(flightId) == null)
        {
            throw new InvalidOperationException($"Flight with Id {flightId} does not exist.");
        }

        await flightRepository.DeleteAsync(flightId);
    }
}
