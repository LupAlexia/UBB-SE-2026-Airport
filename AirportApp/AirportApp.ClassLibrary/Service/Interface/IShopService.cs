using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IShopService
{
    Task<IEnumerable<Shop>> GetAllAvailableShopsAsync();
    Task<Shop?> GetShopByIdAsync(int shopId);
    Task AddShopAsync(Shop shop);
    Task UpdateShopAsync(Shop shop);
    Task DeleteShopAsync(int shopId);
    Task<IEnumerable<Shop>> SortAlphabeticallyAsync();
    Task<IEnumerable<Shop>> SearchByNameAsync(string name);
}
