using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IShopItemRepository
{
    Task<IEnumerable<ShopItem>> GetAsync();
    Task<ShopItem?> GetByIdAsync(int shopItemId);
    Task<int> AddAsync(ShopItem shopItem);
    Task UpdateAsync(ShopItem shopItem);
    Task DeleteAsync(int shopItemId);
}