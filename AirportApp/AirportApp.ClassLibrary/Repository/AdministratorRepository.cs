using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class AdministratorRepository(AppDbContext databaseContext) : IAdministratorRepository
{
    public async Task<IEnumerable<Administrator>> GetAsync()
    {
        return await databaseContext.Senders
            .OfType<Administrator>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Administrator?> GetByIdAsync(int administratorId)
    {
        var administrator = await databaseContext.Senders
            .OfType<Administrator>()
            .FirstOrDefaultAsync(admin => admin.Id == administratorId);

        if (administrator is null)
        {
            throw new KeyNotFoundException($"Administrator with id {administratorId} was not found.");
        }

        return administrator;
    }

    public async Task<int> AddAsync(Administrator administrator)
    {
        if (administrator is null)
        {
            throw new ArgumentNullException(nameof(administrator));
        }

        administrator.Id = 0;
        databaseContext.Senders.Add(administrator);
        await databaseContext.SaveChangesAsync();

        return administrator.Id;
    }

    public async Task UpdateAsync(Administrator administrator)
    {
        if (administrator is null)
        {
            throw new ArgumentNullException(nameof(administrator));
        }

        var existingAdministrator = await databaseContext.Senders
            .OfType<Administrator>()
            .FirstOrDefaultAsync(admin => admin.Id == administrator.Id);

        if (existingAdministrator is not null)
        {
            existingAdministrator.FullName = administrator.FullName;
            existingAdministrator.EmailAddress = administrator.EmailAddress;

            await databaseContext.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int administratorId)
    {
        var administratorToRemove = await databaseContext.Senders
            .OfType<Administrator>()
            .FirstOrDefaultAsync(admin => admin.Id == administratorId);

        if (administratorToRemove is not null)
        {
            databaseContext.Senders.Remove(administratorToRemove);
            await databaseContext.SaveChangesAsync();
        }
    }
}