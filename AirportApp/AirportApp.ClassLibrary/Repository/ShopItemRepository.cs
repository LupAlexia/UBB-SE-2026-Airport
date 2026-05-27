using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class ShopItemRepository(AppDbContext databaseContext) : IShopItemRepository
{
    public async Task<IEnumerable<ShopItem>> GetAsync()
    {
        return await databaseContext.ShopItems
            .Include(shopItem => shopItem.Shop)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ShopItem?> GetByIdAsync(int shopItemId)
    {
        return await databaseContext.ShopItems
            .Include(shopItem => shopItem.Shop)
            .FirstOrDefaultAsync(shopItem => shopItem.Id == shopItemId);
    }

    public async Task<int> AddAsync(ShopItem shopItem)
    {
        if (shopItem is null)
        {
            throw new ArgumentNullException(nameof(shopItem));
        }

        if (shopItem.Shop is not null)
        {
            var existingShop = await databaseContext.Shops.FindAsync(shopItem.Shop.Id);

            if (existingShop is null)
            {
                throw new InvalidOperationException($"Cannot add item: Shop with ID {shopItem.Shop.Id} does not exist.");
            }

            shopItem.Shop = existingShop;
        }

        shopItem.Id = 0;
        databaseContext.ShopItems.Add(shopItem);
        await databaseContext.SaveChangesAsync();

        return shopItem.Id;
    }

    public async Task UpdateAsync(ShopItem shopItem)
    {
        if (shopItem is null)
        {
            throw new ArgumentNullException(nameof(shopItem));
        }

        var existingItem = await databaseContext.ShopItems.FindAsync(shopItem.Id);
        if (existingItem is null)
        {
            return;
        }

        existingItem.Quantity = shopItem.Quantity;
        existingItem.Price = shopItem.Price;
        existingItem.Photo = shopItem.Photo;
        existingItem.Name = shopItem.Name;
        existingItem.Description = shopItem.Description;

        if (shopItem.Shop is not null)
        {
            var existingShop = await databaseContext.Shops.FindAsync(shopItem.Shop.Id);
            if (existingShop is not null)
            {
                existingItem.Shop = existingShop;
            }
        }

        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int shopItemId)
    {
        var itemToRemove = await databaseContext.ShopItems.FindAsync(shopItemId);

        if (itemToRemove is null)
        {
            return;
        }

        databaseContext.ShopItems.Remove(itemToRemove);
        await databaseContext.SaveChangesAsync();
    }
}