using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class AirportService(IAirportRepository airportRepository, IFlightRepository flightRepository) : IAirportService
{
    public async Task<IEnumerable<Airport>> GetAllAirportsAsync()
    {
        return await airportRepository.GetAsync();
    }

    public async Task<Airport?> GetAirportByIdAsync(int airportId)
    {
        return await airportRepository.GetByIdAsync(airportId);
    }

    public async Task AddAirportAsync(Airport airport)
    {
        if (string.IsNullOrWhiteSpace(airport.AirportCode))
            throw new ArgumentException("Airport code cannot be empty.");
        if (string.IsNullOrWhiteSpace(airport.Name))
            throw new ArgumentException("Airport name cannot be empty.");
        if (string.IsNullOrWhiteSpace(airport.City))
            throw new ArgumentException("City cannot be empty.");

        await airportRepository.AddAsync(airport);
    }

    public async Task UpdateAirportAsync(Airport airport)
    {
        var existing = await airportRepository.GetByIdAsync(airport.Id);
        if (existing is null)
            throw new InvalidOperationException($"Airport with ID {airport.Id} not found.");

        if (airport.AirportCode is not null)
            existing.AirportCode = airport.AirportCode;
        if (airport.Name is not null)
            existing.Name = airport.Name;
        if (airport.City is not null)
            existing.City = airport.City;

        await airportRepository.UpdateAsync(existing);
    }

    public async Task DeleteAirportAsync(int airportId)
    {
        await airportRepository.DeleteAsync(airportId);
    }

    public async Task<bool> HasFlightsAsync(int airportId)
    {
        var flights = await flightRepository.GetByAirportIdAsync(airportId);
        return flights.Any();
    }

    public async Task<string> GetDeleteWarningMessageAsync(int airportId, string airportName)
    {
        bool hasFlights = await HasFlightsAsync(airportId);
        if (hasFlights)
            return $"CRITICAL: Airport '{airportName}' has flights assigned. Deleting it will remove all associated flights. Are you sure?";
        return $"Are you sure you want to delete airport '{airportName}'?";
    }

    public async Task SaveAirportAsync(Airport airport)
    {
        if (airport.Id == 0)
            await AddAirportAsync(airport);
        else
            await UpdateAirportAsync(airport);
    }
}
