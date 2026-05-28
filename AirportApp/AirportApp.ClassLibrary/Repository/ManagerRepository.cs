using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class ManagerRepository(AppDbContext databaseContext) : IManagerRepository
{
    public async Task<IEnumerable<Manager>> GetAsync()
    {
        return await databaseContext.Managers
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Manager?> GetByIdAsync(int managerId)
    {
        return await databaseContext.Managers.FindAsync(managerId);
    }

    public async Task<int> AddAsync(Manager manager)
    {
        if (manager is null)
        {
            throw new ArgumentNullException(nameof(manager));
        }

        manager.Id = 0;
        databaseContext.Managers.Add(manager);
        await databaseContext.SaveChangesAsync();

        return manager.Id;
    }

    public async Task UpdateAsync(Manager manager)
    {
        if (manager is null)
        {
            throw new ArgumentNullException(nameof(manager));
        }

        var existingManager = await databaseContext.Managers.FindAsync(manager.Id);
        if (existingManager is null)
        {
            return;
        }

        existingManager.Name = manager.Name;
        existingManager.Email = manager.Email;
        existingManager.Phone = manager.Phone;

        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int managerId)
    {
        var managerToRemove = await databaseContext.Managers.FindAsync(managerId);

        if (managerToRemove is null)
        {
            return;
        }

        databaseContext.Managers.Remove(managerToRemove);
        await databaseContext.SaveChangesAsync();
    }
}