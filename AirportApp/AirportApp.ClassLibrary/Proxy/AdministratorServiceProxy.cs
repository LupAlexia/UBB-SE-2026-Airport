using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class AdministratorServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IAdministratorService
{
    private const string BaseUrl = "api/administrator";

    public async Task<Administrator?> GetAdministratorByIdAsync(int identificationNumber)
    {
        var dto = await GetOptionalAsync<AdministratorDTO>($"{BaseUrl}/{identificationNumber}");
        return dto is null ? null : MapToEntity(dto);
    }

    public async Task<int> AddAdministratorAsync(Administrator administratorEntity)
    {
        return await PostForResultAsync<AdministratorDTO, int>(BaseUrl, MapToDto(administratorEntity));
    }

    public async Task UpdateAdministratorByIdAsync(int identificationNumber, Administrator administratorEntity)
    {
        await PutAsync($"{BaseUrl}/{identificationNumber}", MapToDto(administratorEntity));
    }

    public async Task DeleteAdministratorByIdAsync(int identificationNumber)
    {
        await DeleteAsync($"{BaseUrl}/{identificationNumber}");
    }

    public async Task<List<Administrator>> GetAllAdministratorsAsync()
    {
        var dtos = await GetListAsync<AdministratorDTO>(BaseUrl);
        return dtos.Select(MapToEntity).ToList();
    }

    public Task CreateNewAdministratorAsync(int identificationNumber, string fullName, string emailAddress)
    {
        throw new NotSupportedException("CreateNewAdministratorAsync is not available through the service proxy.");
    }

    public Task ValidateAdministratorIntegrityAsync(Administrator administratorEntity)
    {
        throw new NotSupportedException("ValidateAdministratorIntegrityAsync is not available through the service proxy.");
    }

    private static Administrator MapToEntity(AdministratorDTO dto)
    {
        return new Administrator(0, dto.name, dto.email);
    }

    private static AdministratorDTO MapToDto(Administrator admin)
    {
        return new AdministratorDTO(admin.FullName, admin.EmailAddress);
    }
}
