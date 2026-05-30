using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class AdministratorServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IAdministratorService
{
    private const string BaseUrl = "api/administrator";

    public async Task<Administrator?> GetAdministratorByIdAsync(int identificationNumber)
    {
        return await GetOptionalAsync<Administrator>($"{BaseUrl}/{identificationNumber}");
    }

    public async Task<int> AddAdministratorAsync(Administrator administratorEntity)
    {
        return await PostForResultAsync<Administrator, int>(BaseUrl, administratorEntity);
    }

    public async Task UpdateAdministratorByIdAsync(int identificationNumber, Administrator administratorEntity)
    {
        await PutAsync($"{BaseUrl}/{identificationNumber}", administratorEntity);
    }

    public async Task DeleteAdministratorByIdAsync(int identificationNumber)
    {
        await DeleteAsync($"{BaseUrl}/{identificationNumber}");
    }

    public async Task<List<Administrator>> GetAllAdministratorsAsync()
    {
        return await GetListAsync<Administrator>(BaseUrl);
    }

    public Task CreateNewAdministratorAsync(int identificationNumber, string fullName, string emailAddress, string departmentName)
    {
        throw new NotSupportedException("CreateNewAdministratorAsync is not available through the service proxy.");
    }

    public Task ValidateAdministratorIntegrityAsync(Administrator administratorEntity)
    {
        throw new NotSupportedException("ValidateAdministratorIntegrityAsync is not available through the service proxy.");
    }
}
