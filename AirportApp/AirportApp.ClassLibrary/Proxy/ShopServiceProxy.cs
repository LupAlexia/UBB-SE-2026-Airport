using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class ShopServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IShopService
{
    private const string BaseUrl = "api/shops";

    public async Task<IEnumerable<Shop>> GetAllAvailableShopsAsync()
    {
        return await GetListAsync<Shop>(BaseUrl);
    }

    public async Task<Shop?> GetShopByIdAsync(int shopId)
    {
        return await GetOptionalAsync<Shop>($"{BaseUrl}/{shopId}");
    }

    public async Task AddShopAsync(Shop shop)
    {
        await PostAsync(BaseUrl, shop);
    }

    public async Task UpdateShopAsync(Shop shop)
    {
        await PutAsync($"{BaseUrl}/{shop.Id}", shop);
    }

    public async Task DeleteShopAsync(int shopId)
    {
        await DeleteAsync($"{BaseUrl}/{shopId}");
    }

    public async Task<IEnumerable<Shop>> SortAlphabeticallyAsync()
    {
        return await GetListAsync<Shop>($"{BaseUrl}/sorted");
    }

    public async Task<IEnumerable<Shop>> SearchByNameAsync(string name)
    {
        return await GetListAsync<Shop>($"{BaseUrl}/search?name={Uri.EscapeDataString(name)}");
    }
}
