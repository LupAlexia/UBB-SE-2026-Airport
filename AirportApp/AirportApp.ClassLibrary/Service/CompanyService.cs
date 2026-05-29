using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class CompanyService(ICompanyRepository companyRepository, IFlightRouteService flightRouteService) : ICompanyService
{
    public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
    {
        return await companyRepository.GetAsync();
    }

    public async Task<Company?> GetCompanyByIdAsync(int companyId)
    {
        return await companyRepository.GetByIdAsync(companyId);
    }

    public async Task AddCompanyAsync(Company company)
    {
        await companyRepository.AddAsync(company);
    }

    public async Task UpdateCompanyAsync(Company company)
    {
        var existing = await companyRepository.GetByIdAsync(company.Id);
        if (existing is null)
            throw new InvalidOperationException($"Company with ID {company.Id} not found.");

        if (company.Name is not null)
            existing.Name = company.Name;

        await companyRepository.UpdateAsync(existing);
    }

    public async Task DeleteCompanyAsync(int companyId)
    {
        await companyRepository.DeleteAsync(companyId);
    }

    public async Task<string> GenerateFlightCodeUsingCompanyIdAsync(int companyId)
    {
        var company = await companyRepository.GetByIdAsync(companyId);
        if (company is null)
            throw new InvalidOperationException($"Company with ID {companyId} not found.");

        string prefix = BuildPrefix(company.Name);
        int sequence = await GetNextSequenceAsync(companyId);
        return $"{prefix}-{sequence}";
    }

    private string BuildPrefix(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            return "FL";

        var words = companyName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length >= 2)
        {
            return string.Concat(words.Select(w => char.ToUpperInvariant(w[0])));
        }

        string name = companyName.Trim();
        if (name.Length >= 2)
            return name.Substring(0, 2).ToUpperInvariant();

        return "FL";
    }

    private async Task<int> GetNextSequenceAsync(int companyId)
    {
        var flights = await flightRouteService.GetFlightsByCompanyIdAsync(companyId);
        int maxNum = 0;
        foreach (var flight in flights)
        {
            var parts = flight.FlightNumber?.Split('-');
            if (parts is not null && parts.Length >= 2 && int.TryParse(parts[^1], out int num))
            {
                if (num > maxNum) maxNum = num;
            }
        }
        return maxNum > 0 ? maxNum + 1 : 1000;
    }

    public async Task<int> ValidateFlightCreationInputsAsync(int companyId, int airportId, string capacityText, int runwayId, int gateId)
    {
        if (companyId <= 0) throw new ArgumentException("Invalid company ID.");
        if (airportId <= 0) throw new ArgumentException("Invalid airport ID.");
        if (gateId <= 0) throw new ArgumentException("Invalid gate ID.");
        if (runwayId <= 0) throw new ArgumentException("Invalid runway ID.");
        if (!int.TryParse(capacityText, out int capacity) || capacity <= 0)
            throw new ArgumentException("Capacity must be a positive integer.");

        await Task.CompletedTask;
        return capacity;
    }
}
