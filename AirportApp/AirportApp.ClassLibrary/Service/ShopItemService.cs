using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ShopItemService(IShopItemRepository shopItemRepository) : IShopItemService
{
    private const string ItemNotFoundErrorMessage = "Shop item with Id {0} does not exist.";
    private const string NegativeQuantityErrorMessage = "Quantity cannot be negative.";
    private const string InvalidPriceErrorMessage = "Price must be greater than zero.";
    private const string EmptyNameErrorMessage = "Shop item name cannot be empty.";
    private const string InvalidShopErrorMessage = "Shop item must have a valid shop Id.";

    public async Task<IEnumerable<ShopItem>> GetAllAsync()
    {
        return await shopItemRepository.GetAsync();
    }

    public async Task<ShopItem?> GetByIdAsync(int shopItemId)
    {
        // Return null when item not found; callers handle not-found cases.
        return await shopItemRepository.GetByIdAsync(shopItemId);
    }

    public async Task<IEnumerable<ShopItem>> GetItemsByShopIdAsync(int shopId)
    {
        IEnumerable<ShopItem> allItems = await shopItemRepository.GetAsync();
        List<ShopItem> filteredItems = new List<ShopItem>();

        foreach (ShopItem item in allItems)
        {
            if (item.Shop != null && item.Shop.Id == shopId)
            {
                filteredItems.Add(item);
            }
        }

        return filteredItems;
    }

    public async Task<IEnumerable<ShopItem>> SearchItemsByNameAsync(int shopId, string searchText)
    {
        string query = searchText ?? string.Empty;
        IEnumerable<ShopItem> shopItems = await GetItemsByShopIdAsync(shopId);
        List<ShopItem> matchingItems = new List<ShopItem>();

        foreach (ShopItem item in shopItems)
        {
            if (item.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                matchingItems.Add(item);
            }
        }

        return matchingItems;
    }

    public async Task RemoveShopItemAsync(int shopItemId)
    {
        await shopItemRepository.DeleteAsync(shopItemId);
    }

    public async Task AddShopItemAsync(ShopItem shopItem)
    {
        ValidateShopItem(shopItem);
        await shopItemRepository.AddAsync(shopItem);
    }

    public async Task UpdateShopItemAsync(ShopItem shopItem)
    {
        ValidateShopItem(shopItem);
        await shopItemRepository.UpdateAsync(shopItem);
    }

    public async Task<IEnumerable<ShopItem>> GetItemsSortedByPriceAsync(Shop currentShop)
    {
        if (currentShop == null)
        {
            throw new ArgumentNullException(nameof(currentShop));
        }

        List<ShopItem> items = (await GetItemsByShopIdAsync(currentShop.Id)).ToList();
        items.Sort(new ShopItemPriceComparer());
        return items;
    }

    public async Task<IEnumerable<ShopItem>> GetItemsSortedAlphabeticallyAsync(Shop currentShop)
    {
        if (currentShop == null)
        {
            throw new ArgumentNullException(nameof(currentShop));
        }

        List<ShopItem> items = (await GetItemsByShopIdAsync(currentShop.Id)).ToList();
        items.Sort(new ShopItemNameComparer());
        return items;
    }

    private void ValidateShopItem(ShopItem shopItem)
    {
        if (shopItem.Shop == null || shopItem.Shop.Id <= 0)
        {
            throw new ArgumentException(InvalidShopErrorMessage, nameof(shopItem));
        }

        if (shopItem.Quantity < 0)
        {
            throw new ArgumentException(NegativeQuantityErrorMessage, nameof(shopItem));
        }

        if (shopItem.Price <= 0)
        {
            throw new ArgumentException(InvalidPriceErrorMessage, nameof(shopItem));
        }

        if (string.IsNullOrWhiteSpace(shopItem.Name))
        {
            throw new ArgumentException(EmptyNameErrorMessage, nameof(shopItem));
        }
    }

    private class ShopItemPriceComparer : IComparer<ShopItem>
    {
        public int Compare(ShopItem? firstItem, ShopItem? secondItem)
        {
            if (firstItem == null || secondItem == null)
            {
                return 0;
            }

            return firstItem.Price.CompareTo(secondItem.Price);
        }
    }

    private class ShopItemNameComparer : IComparer<ShopItem>
    {
        public int Compare(ShopItem? firstItem, ShopItem? secondItem)
        {
            if (firstItem == null || secondItem == null)
            {
                return 0;
            }

            return string.Compare(firstItem.Name, secondItem.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
