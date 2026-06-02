using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class CompanyServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), ICompanyService
{
    private const string BaseUrl = "api/companies";

    public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
    {
        var dtos = await GetListAsync<CompanyDTO>(BaseUrl);
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<Company?> GetCompanyByIdAsync(int companyId)
    {
        var dto = await GetOptionalAsync<CompanyDTO>($"{BaseUrl}/{companyId}");
        return dto is null ? null : MapToEntity(dto);
    }

    public async Task AddCompanyAsync(Company company)
    {
        await PostAsync(BaseUrl, MapToDto(company));
    }

    public async Task UpdateCompanyAsync(Company company)
    {
        await PutAsync($"{BaseUrl}/{company.Id}", MapToDto(company));
    }

    public async Task DeleteCompanyAsync(int companyId)
    {
        await DeleteAsync($"{BaseUrl}/{companyId}");
    }

    public async Task<IEnumerable<Company>> GetCompaniesByManagerIdAsync(int managerId)
    {
        var dtos = await GetListAsync<CompanyDTO>($"{BaseUrl}/manager/{managerId}");
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<string> GenerateFlightCodeUsingCompanyIdAsync(int companyId)
    {
        return await GetRequiredAsync<string>($"{BaseUrl}/{companyId}/flight-code");
    }

    public async Task<int> ValidateFlightCreationInputsAsync(int companyId, int airportId, string capacityText, int runwayId, int gateId)
    {
        return await GetRequiredAsync<int>($"{BaseUrl}/validate-flight-inputs?companyId={companyId}&airportId={airportId}&capacityText={Uri.EscapeDataString(capacityText)}&runwayId={runwayId}&gateId={gateId}");
    }

    private static Company MapToEntity(CompanyDTO dto)
    {
        return new Company(dto.id, dto.name);
    }

    private static CompanyDTO MapToDto(Company company)
    {
        return new CompanyDTO(company.Id, company.Name);
    }
}
