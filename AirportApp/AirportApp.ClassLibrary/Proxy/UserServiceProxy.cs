using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class UserServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IUserService
{
    private const string BaseUrl = "api/user";

    public async Task<User> GetByIdAsync(int identificationNumber)
    {
        return await GetRequiredAsync<User>($"{BaseUrl}/{identificationNumber}");
    }

    public async Task<int> AddUserAsync(User userEntity)
    {
        return await PostForResultAsync<UserDTO, int>(BaseUrl, MapToDto(userEntity));
    }

    public async Task UpdateUserByIdAsync(int identificationNumber, User userEntity)
    {
        await PutAsync($"{BaseUrl}/{identificationNumber}", MapToDto(userEntity));
    }

    public async Task DeleteUserByIdAsync(int identificationNumber)
    {
        await DeleteAsync($"{BaseUrl}/{identificationNumber}");
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await GetListAsync<User>(BaseUrl);
    }

    public Task CreateNewUserAsync(int identificationNumber, string fullName, string emailAddress)
    {
        throw new NotSupportedException("CreateNewUserAsync is not available through the service proxy.");
    }

    public Task ValidateUserIntegrityAsync(User userEntity)
    {
        throw new NotSupportedException("ValidateUserIntegrityAsync is not available through the service proxy.");
    }

    private static UserDTO MapToDto(User user)
    {
        return new UserDTO(user.FullName, user.EmailAddress);
    }
}
