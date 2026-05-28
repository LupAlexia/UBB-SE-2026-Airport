using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IAirportService
{
    Task<IEnumerable<Airport>> GetAllAirportsAsync();
    Task<Airport?> GetAirportByIdAsync(int airportId);
    Task AddAirportAsync(Airport airport);
    Task UpdateAirportAsync(Airport airport);
    Task DeleteAirportAsync(int airportId);
    Task<bool> HasFlightsAsync(int airportId);
    Task<string> GetDeleteWarningMessageAsync(int airportId, string airportName);
    Task SaveAirportAsync(Airport airport);
}
