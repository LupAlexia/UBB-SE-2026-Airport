using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await userRepository.GetAsync();
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        return await userRepository.GetByIdAsync(userId);
    }

    public async Task AddAsync(User user)
    {
        await userRepository.AddAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        await userRepository.UpdateAsync(user);
    }

    public async Task DeleteAsync(int userId)
    {
        await userRepository.DeleteAsync(userId);
    }
}
