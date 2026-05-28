using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class UserRepository(AppDbContext databaseContext) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAsync()
    {
        return await databaseContext.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        var user = await databaseContext.Users.FindAsync(userId);

        if (user is null)
        {
            throw new KeyNotFoundException($"User with id {userId} was not found.");
        }

        return user;
    }

    public async Task<int> AddAsync(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        user.Id = 0;
        databaseContext.Users.Add(user);
        await databaseContext.SaveChangesAsync();

        return user.Id;
    }

    public async Task UpdateAsync(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        databaseContext.Users.Update(user);
        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId)
    {
        var userToRemove = await databaseContext.Users.FindAsync(userId);

        if (userToRemove is not null)
        {
            databaseContext.Users.Remove(userToRemove);
            await databaseContext.SaveChangesAsync();
        }
    }
}