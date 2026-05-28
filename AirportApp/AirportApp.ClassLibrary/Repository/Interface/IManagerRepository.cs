using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IManagerRepository
{
    Task<IEnumerable<Manager>> GetAsync();
    Task<Manager?> GetByIdAsync(int managerId);
    Task<int> AddAsync(Manager manager);
    Task UpdateAsync(Manager manager);
    Task DeleteAsync(int managerId);
}