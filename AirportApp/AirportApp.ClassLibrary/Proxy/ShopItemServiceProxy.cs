using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class ShopItemServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IShopItemService
{
    private const string BaseUrl = "api/shopitems";

    public async Task<IEnumerable<ShopItem>> GetAllAsync()
    {
        return await GetListAsync<ShopItem>(BaseUrl);
    }

    public async Task<ShopItem> GetByIdAsync(int shopItemId)
    {
        return await GetRequiredAsync<ShopItem>($"{BaseUrl}/{shopItemId}");
    }

    public async Task<IEnumerable<ShopItem>> GetItemsByShopIdAsync(int shopId)
    {
        return await GetListAsync<ShopItem>($"{BaseUrl}/shop/{shopId}");
    }

    public async Task<IEnumerable<ShopItem>> SearchItemsByNameAsync(int shopId, string searchText)
    {
        return await GetListAsync<ShopItem>($"{BaseUrl}/shop/{shopId}/search?searchText={Uri.EscapeDataString(searchText)}");
    }

    public async Task AddShopItemAsync(ShopItem shopItem)
    {
        await PostAsync(BaseUrl, shopItem);
    }

    public async Task UpdateShopItemAsync(ShopItem shopItem)
    {
        await PutAsync($"{BaseUrl}/{shopItem.Id}", shopItem);
    }

    public async Task RemoveShopItemAsync(int shopItemId)
    {
        await DeleteAsync($"{BaseUrl}/{shopItemId}");
    }

    public async Task<IEnumerable<ShopItem>> GetItemsSortedByPriceAsync(Shop currentShop)
    {
        return await GetListAsync<ShopItem>($"{BaseUrl}/shop/{currentShop.Id}/sorted-by-price");
    }

    public async Task<IEnumerable<ShopItem>> GetItemsSortedAlphabeticallyAsync(Shop currentShop)
    {
        return await GetListAsync<ShopItem>($"{BaseUrl}/shop/{currentShop.Id}/sorted-alphabetically");
    }
}
