using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IShopItemService
{
    Task<IEnumerable<ShopItem>> GetAllAsync();
    Task<ShopItem> GetByIdAsync(int shopItemId);
    Task<IEnumerable<ShopItem>> GetItemsByShopIdAsync(int shopId);
    Task<IEnumerable<ShopItem>> SearchItemsByNameAsync(string name);
    Task AddShopItemAsync(ShopItem shopItem);
    Task UpdateShopItemAsync(ShopItem shopItem);
    Task RemoveShopItemAsync(int shopItemId);
    Task<IEnumerable<ShopItem>> GetItemsSortedByPriceAsync();
    Task<IEnumerable<ShopItem>> GetItemsSortedAlphabeticallyAsync();
}
