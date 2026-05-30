using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class RunwayServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IRunwayService
{
    private const string BaseUrl = "api/runways";

    public async Task<IEnumerable<Runway>> GetAllRunwaysAsync()
    {
        return await GetListAsync<Runway>(BaseUrl);
    }

    public async Task<Runway?> GetRunwayByIdAsync(int runwayId)
    {
        return await GetOptionalAsync<Runway>($"{BaseUrl}/{runwayId}");
    }

    public async Task AddRunwayAsync(Runway runway)
    {
        await PostAsync(BaseUrl, runway);
    }

    public async Task UpdateRunwayAsync(Runway runway)
    {
        await PutAsync($"{BaseUrl}/{runway.Id}", runway);
    }

    public async Task DeleteRunwayAsync(int runwayId)
    {
        await DeleteAsync($"{BaseUrl}/{runwayId}");
    }

    public async Task<bool> HasFlightsAsync(int runwayId)
    {
        return await GetRequiredAsync<bool>($"{BaseUrl}/{runwayId}/has-flights");
    }

    public async Task SaveRunwayAsync(int runwayId, string name, string handleTimeText)
    {
        var payload = new { RunwayId = runwayId, Name = name, HandleTimeText = handleTimeText };
        await PostAsync($"{BaseUrl}/save", payload);
    }

    public async Task<string> GetDeleteWarningMessageAsync(int runwayId)
    {
        var response = await GetRequiredAsync<DeleteWarningResponse>($"{BaseUrl}/{runwayId}/delete-warning");
        return response.WarningMessage;
    }

    public record DeleteWarningResponse(string WarningMessage);
}
