using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class CompanyServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), ICompanyService
{
    private const string BaseUrl = "api/companies";

    public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
    {
        return await GetListAsync<Company>(BaseUrl);
    }

    public async Task<Company?> GetCompanyByIdAsync(int companyId)
    {
        return await GetOptionalAsync<Company>($"{BaseUrl}/{companyId}");
    }

    public async Task AddCompanyAsync(Company company)
    {
        await PostAsync(BaseUrl, company);
    }

    public async Task UpdateCompanyAsync(Company company)
    {
        await PutAsync($"{BaseUrl}/{company.Id}", company);
    }

    public async Task DeleteCompanyAsync(int companyId)
    {
        await DeleteAsync($"{BaseUrl}/{companyId}");
    }

    public async Task<string> GenerateFlightCodeUsingCompanyIdAsync(int companyId)
    {
        return await GetRequiredAsync<string>($"{BaseUrl}/{companyId}/flight-code");
    }

    public async Task<int> ValidateFlightCreationInputsAsync(int companyId, int airportId, string capacityText, int runwayId, int gateId)
    {
        return await GetRequiredAsync<int>($"{BaseUrl}/validate-flight-inputs?companyId={companyId}&airportId={airportId}&capacityText={Uri.EscapeDataString(capacityText)}&runwayId={runwayId}&gateId={gateId}");
    }
}
