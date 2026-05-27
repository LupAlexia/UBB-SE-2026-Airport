using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IGateRepository
{
    Task<IEnumerable<Gate>> GetAsync();
    Task<Gate?> GetByIdAsync(int gateId);
    Task<int> AddAsync(Gate gate);
    Task UpdateAsync(Gate gate);
    Task DeleteAsync(int gateId);
}