using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IRunwayRepository
{
    Task<IEnumerable<Runway>> GetAsync();
    Task<Runway?> GetByIdAsync(int runwayId);
    Task<int> AddAsync(Runway runway);
    Task UpdateAsync(Runway runway);
    Task DeleteAsync(int runwayId);
}