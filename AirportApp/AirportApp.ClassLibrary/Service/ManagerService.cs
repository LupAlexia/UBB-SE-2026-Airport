using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ManagerService(IManagerRepository managerRepository) : IManagerService
{
    public async Task<IEnumerable<Manager>> GetAllManagersAsync()
    {
        return await managerRepository.GetAsync();
    }

    public async Task<Manager?> GetManagerByIdAsync(int managerId)
    {
        return await managerRepository.GetByIdAsync(managerId);
    }

    public async Task AddManagerAsync(Manager manager)
    {
        if (string.IsNullOrWhiteSpace(manager.Name))
            throw new ArgumentException("Manager name cannot be empty.");
        if (string.IsNullOrWhiteSpace(manager.Email) || !manager.Email.Contains('@'))
            throw new ArgumentException("A valid email address is required.");
        if (string.IsNullOrWhiteSpace(manager.Phone))
            throw new ArgumentException("Phone number cannot be empty.");

        await managerRepository.AddAsync(manager);
    }

    public async Task UpdateManagerAsync(Manager manager)
    {
        if (string.IsNullOrWhiteSpace(manager.Name))
            throw new ArgumentException("Manager name cannot be empty.");
        if (string.IsNullOrWhiteSpace(manager.Email) || !manager.Email.Contains('@'))
            throw new ArgumentException("A valid email address is required.");
        if (string.IsNullOrWhiteSpace(manager.Phone))
            throw new ArgumentException("Phone number cannot be empty.");

        await managerRepository.UpdateAsync(manager);
    }

    public async Task DeleteManagerAsync(int managerId)
    {
        await managerRepository.DeleteAsync(managerId);
    }

    public async Task<Manager> GetAnyManagerAsync()
    {
        var managers = await managerRepository.GetAsync();
        return managers.FirstOrDefault()
            ?? throw new InvalidOperationException("No managers found.");
    }
}
