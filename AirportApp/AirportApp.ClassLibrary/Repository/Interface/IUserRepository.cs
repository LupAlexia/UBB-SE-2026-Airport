using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAsync();
    Task<User?> GetByIdAsync(int userId);
    Task<int> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int userId);
}