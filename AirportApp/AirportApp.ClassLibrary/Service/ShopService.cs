using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ShopService(IShopRepository shopRepository) : IShopService
{
    public async Task<IEnumerable<Shop>> GetAllAvailableShopsAsync()
    {
        return await shopRepository.GetAsync();
    }

    public async Task<Shop?> GetShopByIdAsync(int shopId)
    {
        return await shopRepository.GetByIdAsync(shopId);
    }

    public async Task AddShopAsync(Shop shop)
    {
        if (string.IsNullOrWhiteSpace(shop.Name))
            throw new ArgumentException("Shop name cannot be empty.");
        if (string.IsNullOrWhiteSpace(shop.Type))
            throw new ArgumentException("Shop type cannot be empty.");

        var allShops = await shopRepository.GetAsync();
        if (allShops.Any(s => s.Name.Equals(shop.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A shop with the name '{shop.Name}' already exists.");

        await shopRepository.AddAsync(shop);
    }

    public async Task UpdateShopAsync(Shop shop)
    {
        if (string.IsNullOrWhiteSpace(shop.Name))
            throw new ArgumentException("Shop name cannot be empty.");
        if (string.IsNullOrWhiteSpace(shop.Type))
            throw new ArgumentException("Shop type cannot be empty.");

        var allShops = await shopRepository.GetAsync();
        if (allShops.Any(s => s.Id != shop.Id && s.Name.Equals(shop.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Another shop with the name '{shop.Name}' already exists.");

        await shopRepository.UpdateAsync(shop);
    }

    public async Task DeleteShopAsync(int shopId)
    {
        await shopRepository.DeleteAsync(shopId);
    }

    public async Task<IEnumerable<Shop>> SortAlphabeticallyAsync()
    {
        var shops = await shopRepository.GetAsync();
        return shops.OrderBy(s => s.Name).ToList();
    }

    public async Task<IEnumerable<Shop>> SearchByNameAsync(string name)
    {
        var shops = await shopRepository.GetAsync();
        if (string.IsNullOrWhiteSpace(name)) return shops;
        return shops.Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
