using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ShopItemService(IShopItemRepository shopItemRepository) : IShopItemService
{
    public async Task<IEnumerable<ShopItem>> GetAllAsync()
    {
        return await shopItemRepository.GetAsync();
    }

    public async Task<ShopItem> GetByIdAsync(int shopItemId)
    {
        var item = await shopItemRepository.GetByIdAsync(shopItemId);
        if (item is null)
            throw new InvalidOperationException($"ShopItem with ID {shopItemId} not found.");
        return item;
    }

    public async Task<IEnumerable<ShopItem>> GetItemsByShopIdAsync(int shopId)
    {
        var all = await shopItemRepository.GetAsync();
        return all.Where(i => i.Shop?.Id == shopId).ToList();
    }

    public async Task<IEnumerable<ShopItem>> SearchItemsByNameAsync(string name)
    {
        var all = await shopItemRepository.GetAsync();
        if (string.IsNullOrWhiteSpace(name)) return all;
        return all.Where(i => i.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task AddShopItemAsync(ShopItem shopItem)
    {
        if (string.IsNullOrWhiteSpace(shopItem.Name))
            throw new ArgumentException("Item name cannot be empty.");
        if (shopItem.Price <= 0)
            throw new ArgumentException("Price must be greater than 0.");
        if (shopItem.Quantity < 0)
            throw new ArgumentException("Quantity cannot be negative.");
        if (shopItem.Shop is null || shopItem.Shop.Id <= 0)
            throw new ArgumentException("A valid shop must be assigned.");

        await shopItemRepository.AddAsync(shopItem);
    }

    public async Task UpdateShopItemAsync(ShopItem shopItem)
    {
        await shopItemRepository.UpdateAsync(shopItem);
    }

    public async Task RemoveShopItemAsync(int shopItemId)
    {
        await shopItemRepository.DeleteAsync(shopItemId);
    }

    public async Task<IEnumerable<ShopItem>> GetItemsSortedByPriceAsync()
    {
        var all = await shopItemRepository.GetAsync();
        return all.OrderBy(i => i.Price).ToList();
    }

    public async Task<IEnumerable<ShopItem>> GetItemsSortedAlphabeticallyAsync()
    {
        var all = await shopItemRepository.GetAsync();
        return all.OrderBy(i => i.Name).ToList();
    }
}
