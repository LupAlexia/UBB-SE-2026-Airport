using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IChatRepository
{
    Task<IEnumerable<Chat>> GetAsync();
    Task<Chat?> GetByIdAsync(int chatId);
    Task<int> AddAsync(Chat chat);
    Task UpdateAsync(Chat chat);
    Task DeleteAsync(int chatId);
}