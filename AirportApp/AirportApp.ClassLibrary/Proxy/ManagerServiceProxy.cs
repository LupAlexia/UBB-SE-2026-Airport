using System;
using System.Collections.Generic;
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
        return await GetListAsync<Manager>(BaseUrl);
    }

    public async Task<Manager?> GetManagerByIdAsync(int managerId)
    {
        return await GetOptionalAsync<Manager>($"{BaseUrl}/{managerId}");
    }

    public async Task AddManagerAsync(Manager manager)
    {
        await PostAsync(BaseUrl, manager);
    }

    public async Task UpdateManagerAsync(Manager manager)
    {
        await PutAsync($"{BaseUrl}/{manager.Id}", manager);
    }

    public async Task DeleteManagerAsync(int managerId)
    {
        await DeleteAsync($"{BaseUrl}/{managerId}");
    }

    public async Task<Manager?> GetAnyManagerAsync()
    {
        return await GetOptionalAsync<Manager>($"{BaseUrl}/any");
    }
}
