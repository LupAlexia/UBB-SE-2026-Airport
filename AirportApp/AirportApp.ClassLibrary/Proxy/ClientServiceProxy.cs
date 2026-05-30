using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class ClientServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IClientService
{
    private const string BaseUrl = "api/clients";

    public async Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        return await GetListAsync<Client>(BaseUrl);
    }

    public async Task<Client?> GetClientByIdAsync(int clientId)
    {
        return await GetOptionalAsync<Client>($"{BaseUrl}/{clientId}");
    }

    public async Task AddClientAsync(Client client)
    {
        await PostAsync(BaseUrl, client);
    }

    public async Task UpdateClientAsync(Client client)
    {
        await PutAsync($"{BaseUrl}/{client.Id}", client);
    }

    public async Task DeleteClientAsync(int clientId)
    {
        await DeleteAsync($"{BaseUrl}/{clientId}");
    }

    public async Task<Client> GetAnyClientAsync()
    {
        return await GetRequiredAsync<Client>($"{BaseUrl}/any");
    }
}
