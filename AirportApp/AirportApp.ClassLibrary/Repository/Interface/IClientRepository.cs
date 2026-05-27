using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAsync();
    Task<Client?> GetByIdAsync(int clientId);
    Task<int> AddAsync(Client client);
    Task UpdateAsync(Client client);
    Task DeleteAsync(int clientId);
}