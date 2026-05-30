using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class AirportServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IAirportService
{
    private const string BaseUrl = "api/airports";

    public async Task<IEnumerable<Airport>> GetAllAirportsAsync()
    {
        var dtos = await GetListAsync<AirportDTO>(BaseUrl);
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<Airport?> GetAirportByIdAsync(int airportId)
    {
        var dto = await GetOptionalAsync<AirportDTO>($"{BaseUrl}/{airportId}");
        return dto is null ? null : MapToEntity(dto);
    }

    public async Task AddAirportAsync(Airport airport)
    {
        await PostAsync(BaseUrl, MapToDto(airport));
    }

    public async Task UpdateAirportAsync(Airport airport)
    {
        await PutAsync($"{BaseUrl}/{airport.Id}", MapToDto(airport));
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
        await PutAsync($"{BaseUrl}/{airport.Id}", MapToDto(airport));
    }

    private static Airport MapToEntity(AirportDTO dto)
    {
        return new Airport(dto.id, dto.airportCode, dto.city, "");
    }

    private static AirportDTO MapToDto(Airport airport)
    {
        return new AirportDTO(airport.Id, airport.AirportCode, airport.City);
    }

    public record DeleteWarningResponse(string WarningMessage);
}
