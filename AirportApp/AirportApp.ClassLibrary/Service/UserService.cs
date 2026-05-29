using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<User> GetByIdAsync(int identificationNumber)
    {
        return await userRepository.GetByIdAsync(identificationNumber) ?? throw new KeyNotFoundException($"User {identificationNumber} not found.");
    }

    public async Task<int> AddUserAsync(User userEntity)
    {
        return await userRepository.AddAsync(userEntity);
    }

    public async Task UpdateUserByIdAsync(int identificationNumber, User userEntity)
    {
        await userRepository.UpdateAsync(userEntity);
    }

    public async Task DeleteUserByIdAsync(int identificationNumber)
    {
        await userRepository.DeleteAsync(identificationNumber);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return (await userRepository.GetAsync()).ToList();
    }

    public async Task CreateNewUserAsync(int identificationNumber, string fullName, string emailAddress)
    {
        User user = new User(identificationNumber, fullName, emailAddress);
        await ValidateUserIntegrityAsync(user);
        await AddUserAsync(user);
    }

    public async Task ValidateUserIntegrityAsync(User userEntity)
    {
        ArgumentNullException.ThrowIfNull(userEntity);
        if ((await this.GetAllUsersAsync()).Contains(userEntity))
            throw new ArgumentException("User already exists");
        if (string.IsNullOrEmpty(userEntity.RetrieveConfiguredDisplayFullNameForBot()))
            throw new ArgumentException("Name cannot be null or empty");
        if (string.IsNullOrEmpty(userEntity.RetrieveConfiguredEmailAddressForBotContact()))
            throw new ArgumentException("Email cannot be null or empty");
    }
}
