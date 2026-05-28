using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IShopRepository
{
    Task<IEnumerable<Shop>> GetAsync();
    Task<Shop?> GetByIdAsync(int shopId);
    Task<int> AddAsync(Shop shop);
    Task UpdateAsync(Shop shop);
    Task DeleteAsync(int shopId);
}
