using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IShopItemService
{
    Task<IEnumerable<ShopItem>> GetAllAsync();
    Task<ShopItem> GetByIdAsync(int shopItemId);
    Task<IEnumerable<ShopItem>> GetItemsByShopIdAsync(int shopId);
    Task<IEnumerable<ShopItem>> SearchItemsByNameAsync(int shopId, string searchText);
    Task AddShopItemAsync(ShopItem shopItem);
    Task UpdateShopItemAsync(ShopItem shopItem);
    Task RemoveShopItemAsync(int shopItemId);
    Task<IEnumerable<ShopItem>> GetItemsSortedByPriceAsync(int shopId);
    Task<IEnumerable<ShopItem>> GetItemsSortedAlphabeticallyAsync(int shopId);
}
