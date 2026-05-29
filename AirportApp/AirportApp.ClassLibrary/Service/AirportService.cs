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
        if (airportId <= 0)
        {
            return null;
        }

        return await airportRepository.GetByIdAsync(airportId);
    }

    public async Task AddAirportAsync(Airport airport)
    {
        if (string.IsNullOrWhiteSpace(airport.AirportCode))
        {
            throw new ArgumentException("The airport code cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(airport.Name))
        {
            throw new ArgumentException("The airport name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(airport.City))
        {
            throw new ArgumentException("The city name cannot be empty.");
        }

        await airportRepository.AddAsync(airport);
    }

    public async Task UpdateAirportAsync(Airport airport)
    {
        Airport? existingAirport = await airportRepository.GetByIdAsync(airport.Id);

        if (existingAirport == null)
        {
            return;
        }

        if (airport.Name != null)
        {
            existingAirport.Name = airport.Name;
        }

        if (airport.City != null)
        {
            existingAirport.City = airport.City;
        }

        if (airport.AirportCode != null)
        {
            existingAirport.AirportCode = airport.AirportCode;
        }

        await airportRepository.UpdateAsync(existingAirport);
    }

    public async Task SaveAirportAsync(Airport airport)
    {
        if (airport.Id == 0)
        {
            await AddAirportAsync(airport);
        }
        else
        {
            await UpdateAirportAsync(airport);
        }
    }

    public async Task DeleteAirportAsync(int airportId)
    {
        if (airportId > 0)
        {
            await airportRepository.DeleteAsync(airportId);
        }
    }

    public async Task<bool> HasFlightsAsync(int airportId)
    {
        IEnumerable<Flight> associatedFlights = await flightRepository.GetByAirportIdAsync(airportId);
        return associatedFlights.Any();
    }

    public async Task<string> GetDeleteWarningMessageAsync(int airportId)
    {
        bool hasFlights = await HasFlightsAsync(airportId);
        Airport? airport = await GetAirportByIdAsync(airportId);

        if (hasFlights)
        {
            return $"CRITICAL: Airport '{airport?.Name}' has flights assigned. Deleting it will remove ALL associated flights. Proceed?";
        }

        return $"Are you sure you want to delete airport '{airport?.Name}'?";
    }
}
