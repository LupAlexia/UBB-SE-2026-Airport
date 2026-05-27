using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface ISenderRepository
{
    Task<IEnumerable<Sender>> GetAsync();
    Task<Sender?> GetByIdAsync(int senderId);
    Task<int> AddAsync(Sender sender);
    Task UpdateAsync(Sender sender);
    Task DeleteAsync(int senderId);
}