using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class AirportServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IAirportService
{
    private const string BaseUrl = "api/airports";

    public async Task<IEnumerable<Airport>> GetAllAirportsAsync()
    {
        return await GetListAsync<Airport>(BaseUrl);
    }

    public async Task<Airport?> GetAirportByIdAsync(int airportId)
    {
        return await GetOptionalAsync<Airport>($"{BaseUrl}/{airportId}");
    }

    public async Task AddAirportAsync(Airport airport)
    {
        await PostAsync(BaseUrl, airport);
    }

    public async Task UpdateAirportAsync(Airport airport)
    {
        await PutAsync($"{BaseUrl}/{airport.Id}", airport);
    }

    public async Task DeleteAirportAsync(int airportId)
    {
        await DeleteAsync($"{BaseUrl}/{airportId}");
    }

    public async Task<bool> HasFlightsAsync(int airportId)
    {
        return await GetRequiredAsync<bool>($"{BaseUrl}/{airportId}/has-flights");
    }

    public async Task<string> GetDeleteWarningMessageAsync(int airportId)
    {
        var response = await GetRequiredAsync<DeleteWarningResponse>($"{BaseUrl}/{airportId}/delete-warning");
        return response.WarningMessage;
    }

    public async Task SaveAirportAsync(Airport airport)
    {
        await PutAsync($"{BaseUrl}/{airport.Id}", airport);
    }

    public record DeleteWarningResponse(string WarningMessage);
}
