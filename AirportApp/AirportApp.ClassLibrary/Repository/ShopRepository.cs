using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class ShopRepository(AppDbContext databaseContext) : IShopRepository
{
    public async Task<IEnumerable<Shop>> GetAsync()
    {
        return await databaseContext.Shops
            .Include(shop => shop.Manager)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Shop?> GetByIdAsync(int shopId)
    {
        return await databaseContext.Shops
            .Include(shop => shop.Manager)
            .FirstOrDefaultAsync(shop => shop.Id == shopId);
    }

    public async Task<int> AddAsync(Shop shop)
    {
        if (shop is null)
        {
            throw new ArgumentNullException(nameof(shop));
        }

        if (shop.Manager is not null)
        {
            var existingManager = await databaseContext.Managers.FindAsync(shop.Manager.Id);
            if (existingManager is null)
            {
                throw new InvalidOperationException($"Cannot add shop: Manager with ID {shop.Manager.Id} does not exist.");
            }
            shop.Manager = existingManager;
        }

        shop.Id = 0;
        databaseContext.Shops.Add(shop);
        await databaseContext.SaveChangesAsync();

        return shop.Id;
    }

    public async Task UpdateAsync(Shop shop)
    {
        if (shop is null)
        {
            throw new ArgumentNullException(nameof(shop));
        }

        var existingShop = await databaseContext.Shops.FindAsync(shop.Id);
        if (existingShop is null)
        {
            return;
        }

        existingShop.Name = shop.Name;
        existingShop.Type = shop.Type;

        if (shop.Manager is not null)
        {
            var existingManager = await databaseContext.Managers.FindAsync(shop.Manager.Id);
            if (existingManager is not null)
            {
                existingShop.Manager = existingManager;
            }
        }

        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int shopId)
    {
        var shopToRemove = await databaseContext.Shops.FindAsync(shopId);

        if (shopToRemove is null)
        {
            return;
        }

        databaseContext.Shops.Remove(shopToRemove);
        await databaseContext.SaveChangesAsync();
    }
}
