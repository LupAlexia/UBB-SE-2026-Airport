using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IAirportRepository
{
    Task<IEnumerable<Airport>> GetAsync();
    Task<Airport?> GetByIdAsync(int airportId);
    Task<int> AddAsync(Airport airport);
    Task UpdateAsync(Airport airport);
    Task DeleteAsync(int airportId);
}