using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface ICompanyService
{
    Task<IEnumerable<Company>> GetAllCompaniesAsync();
    Task<Company?> GetCompanyByIdAsync(int companyId);
    Task<IEnumerable<Company>> GetCompaniesByManagerIdAsync(int managerId);
    Task AddCompanyAsync(Company company);
    Task UpdateCompanyAsync(Company company);
    Task DeleteCompanyAsync(int companyId);
    Task<string> GenerateFlightCodeUsingCompanyIdAsync(int companyId);
    Task<int> ValidateFlightCreationInputsAsync(int companyId, int airportId, string capacityText, int runwayId, int gateId);
}
