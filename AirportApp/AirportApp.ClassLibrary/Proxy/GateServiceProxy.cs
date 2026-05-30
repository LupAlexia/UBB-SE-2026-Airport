using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class GateServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IGateService
{
    private const string BaseUrl = "api/gates";

    public async Task<IEnumerable<Gate>> GetAllGatesAsync()
    {
        return await GetListAsync<Gate>(BaseUrl);
    }

    public async Task<Gate?> GetGateByIdAsync(int gateId)
    {
        return await GetOptionalAsync<Gate>($"{BaseUrl}/{gateId}");
    }

    public async Task AddGateAsync(Gate gate)
    {
        await PostAsync(BaseUrl, gate);
    }

    public async Task UpdateGateAsync(Gate gate)
    {
        await PutAsync($"{BaseUrl}/{gate.Id}", gate);
    }

    public async Task DeleteGateAsync(int gateId)
    {
        await DeleteAsync($"{BaseUrl}/{gateId}");
    }

    public async Task<bool> HasFlightsAsync(int gateId)
    {
        return await GetRequiredAsync<bool>($"{BaseUrl}/{gateId}/has-flights");
    }

    public async Task<string> GetDeleteWarningMessageAsync(int gateId)
    {
        var response = await GetRequiredAsync<DeleteWarningResponse>($"{BaseUrl}/{gateId}/delete-warning");
        return response.WarningMessage;
    }

    public async Task SaveGateAsync(Gate gate)
    {
        await PutAsync($"{BaseUrl}/{gate.Id}", gate);
    }

    public record DeleteWarningResponse(string WarningMessage);
}
