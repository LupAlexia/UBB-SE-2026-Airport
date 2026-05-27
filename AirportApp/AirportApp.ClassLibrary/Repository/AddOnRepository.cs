using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class AddOnRepository(AppDbContext databaseContext) : IAddOnRepository
{
    public async Task<IEnumerable<AddOn>> GetAsync()
    {
        return await databaseContext.AddOns
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<AddOn>> GetByIdsAsync(IEnumerable<int> addOnIds)
    {
        if (addOnIds is null)
        {
            throw new ArgumentNullException(nameof(addOnIds));
        }

        return await databaseContext.AddOns
            .Where(addOn => addOnIds.Contains(addOn.Id))
            .AsNoTracking()
            .ToListAsync();
    }
}