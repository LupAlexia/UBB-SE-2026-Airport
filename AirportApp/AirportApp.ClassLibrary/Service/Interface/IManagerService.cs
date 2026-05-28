using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IManagerService
{
    Task<IEnumerable<Manager>> GetAllManagersAsync();
    Task<Manager?> GetManagerByIdAsync(int managerId);
    Task AddManagerAsync(Manager manager);
    Task UpdateManagerAsync(Manager manager);
    Task DeleteManagerAsync(int managerId);
    Task<Manager> GetAnyManagerAsync();
}
