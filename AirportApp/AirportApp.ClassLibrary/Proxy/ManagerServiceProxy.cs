using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class ManagerServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IManagerService
{
    private const string BaseUrl = "api/managers";

    public async Task<IEnumerable<Manager>> GetAllManagersAsync()
    {
        var dtos = await GetListAsync<ManagerDto>(BaseUrl);
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<Manager?> GetManagerByIdAsync(int managerId)
    {
        var dto = await GetOptionalAsync<ManagerDto>($"{BaseUrl}/{managerId}");
        return dto == null ? null : MapToEntity(dto);
    }

    public async Task AddManagerAsync(Manager manager)
    {
        var dto = MapToDto(manager);
        await PostAsync(BaseUrl, dto);
    }

    public async Task UpdateManagerAsync(Manager manager)
    {
        var dto = MapToDto(manager);
        await PutAsync($"{BaseUrl}/{manager.Id}", dto);
    }

    public async Task DeleteManagerAsync(int managerId)
    {
        await DeleteAsync($"{BaseUrl}/{managerId}");
    }

    public async Task<Manager?> GetAnyManagerAsync()
    {
        var dtos = await GetListAsync<ManagerDto>(BaseUrl);
        var first = dtos.FirstOrDefault();
        if (first == null)
        {
            throw new InvalidOperationException("No managers are currently registered in the system.");
        }
        return MapToEntity(first);
    }

    private static Manager MapToEntity(ManagerDto dto)
    {
        return new Manager(dto.Id, dto.Name ?? string.Empty, dto.Email ?? string.Empty, dto.Phone ?? string.Empty);
    }

    private static ManagerDto MapToDto(Manager manager)
    {
        return new ManagerDto
        {
            Id = manager.Id,
            Name = manager.Name,
            Email = manager.Email,
            Phone = manager.Phone
        };
    }

    private sealed class ManagerDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
