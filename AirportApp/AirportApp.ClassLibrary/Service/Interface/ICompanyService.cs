using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface ICompanyService
{
    Task<IEnumerable<Company>> GetAllCompaniesAsync();
    Task<Company?> GetCompanyByIdAsync(int companyId);
    Task AddCompanyAsync(Company company);
    Task UpdateCompanyAsync(Company company);
    Task DeleteCompanyAsync(int companyId);
    Task<string> GenerateFlightCodeUsingCompanyIdAsync(int companyId);
    Task ValidateFlightCreationInputsAsync(int companyId, int airportId, int gateId, int runwayId, string capacityText);
}
