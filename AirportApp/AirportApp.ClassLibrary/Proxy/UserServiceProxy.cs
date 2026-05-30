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
        var dto = await GetRequiredAsync<UserDTO>($"{BaseUrl}/{identificationNumber}");
        return MapToEntity(dto);
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
        var dtos = await GetListAsync<UserDTO>(BaseUrl);
        return dtos.Select(MapToEntity).ToList();
    }

    public Task CreateNewUserAsync(int identificationNumber, string fullName, string emailAddress)
    {
        throw new NotSupportedException("CreateNewUserAsync is not available through the service proxy.");
    }

    public Task ValidateUserIntegrityAsync(User userEntity)
    {
        throw new NotSupportedException("ValidateUserIntegrityAsync is not available through the service proxy.");
    }

    private static User MapToEntity(UserDTO dto)
    {
        return new User(0, dto.name, dto.email);
    }

    private static UserDTO MapToDto(User user)
    {
        return new UserDTO(user.FullName, user.EmailAddress);
    }
}
