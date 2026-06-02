using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class CompanyService(ICompanyRepository companyRepository, IFlightRouteService flightRouteService) : ICompanyService
{
    private const int StartingFlightSequenceNumber = 1000;
    private const string DefaultFlightIdentificationPrefix = "FL";
    private const char FlightCodeDelimiter = '-';
    private const int RequiredWordsForInitials = 2;
    private const int MinimumPrefixLength = 2;

    public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
    {
        return await companyRepository.GetAsync();
    }

    public async Task<Company?> GetCompanyByIdAsync(int companyId)
    {
        if (companyId <= 0)
        {
            return null;
        }

        return await companyRepository.GetByIdAsync(companyId);
    }

    public async Task<IEnumerable<Company>> GetCompaniesByManagerIdAsync(int managerId)
    {
        if (managerId <= 0)
        {
            return Enumerable.Empty<Company>();
        }

        return await companyRepository.GetAllByManagerIdAsync(managerId);
    }

    public async Task AddCompanyAsync(Company company)
    {
        if (string.IsNullOrWhiteSpace(company.Name))
        {
            throw new ArgumentException("The company name cannot be empty.");
        }

        await companyRepository.AddAsync(company);
    }

    public async Task<int> ValidateFlightCreationInputsAsync(int companyId, int airportId, string capacityText, int runwayId, int gateId)
    {
        if (companyId <= 0)
        {
            throw new InvalidOperationException("A company must be selected before adding a flight.");
        }

        if (airportId <= 0 || runwayId <= 0 || gateId <= 0)
        {
            throw new InvalidOperationException("Please ensure all required fields are populated.");
        }

        if (!int.TryParse(capacityText, out int parsedCapacity))
        {
            throw new InvalidOperationException("The provided capacity value is invalid.");
        }

        await Task.CompletedTask;
        return parsedCapacity;
    }

    public async Task<string> GenerateFlightCodeUsingCompanyIdAsync(int companyId)
    {
        Company? company = await companyRepository.GetByIdAsync(companyId);
        string characterPrefix = DetermineFlightPrefix(company);

        IEnumerable<Flight> existingFlights = await flightRouteService.GetFlightsByCompanyIdAsync(companyId);
        int nextSequenceNumber = CalculateNextAvailableFlightNumber(existingFlights);

        return $"{characterPrefix}{FlightCodeDelimiter}{nextSequenceNumber}";
    }

    private string DetermineFlightPrefix(Company? company)
    {
        if (company == null || string.IsNullOrWhiteSpace(company.Name))
        {
            return DefaultFlightIdentificationPrefix;
        }

        string[] nameWords = company.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (nameWords.Length >= RequiredWordsForInitials)
        {
            string firstInitial = nameWords[0][0].ToString();
            string secondInitial = nameWords[1][0].ToString();
            return (firstInitial + secondInitial).ToUpper();
        }

        if (company.Name.Length >= MinimumPrefixLength)
        {
            return company.Name.Substring(0, MinimumPrefixLength).ToUpper();
        }

        return company.Name.ToUpper();
    }

    private int CalculateNextAvailableFlightNumber(IEnumerable<Flight> existingFlights)
    {
        int maxFlightNumberFound = 0;

        if (existingFlights == null)
        {
            return StartingFlightSequenceNumber;
        }

        foreach (Flight flight in existingFlights)
        {
            int extractedNumber = ExtractNumericPartFromFlightCode(flight.FlightNumber);

            if (extractedNumber > maxFlightNumberFound)
            {
                maxFlightNumberFound = extractedNumber;
            }
        }

        if (maxFlightNumberFound >= StartingFlightSequenceNumber)
        {
            return maxFlightNumberFound + 1;
        }

        return StartingFlightSequenceNumber;
    }

    private int ExtractNumericPartFromFlightCode(string? flightNumber)
    {
        if (string.IsNullOrEmpty(flightNumber) || !flightNumber.Contains(FlightCodeDelimiter))
        {
            return 0;
        }

        string[] codeParts = flightNumber.Split(FlightCodeDelimiter);
        string lastSegment = codeParts[codeParts.Length - 1];

        if (int.TryParse(lastSegment, out int parsedFlightNumber))
        {
            return parsedFlightNumber;
        }

        return 0;
    }

    public async Task UpdateCompanyAsync(Company company)
    {
        Company? existingCompany = await companyRepository.GetByIdAsync(company.Id);

        if (existingCompany == null)
        {
            return;
        }

        if (company.Name != null)
        {
            if (string.IsNullOrWhiteSpace(company.Name))
            {
                throw new ArgumentException("The new company name cannot be empty.");
            }

            existingCompany.Name = company.Name;
        }

        await companyRepository.UpdateAsync(existingCompany);
    }

    public async Task DeleteCompanyAsync(int companyId)
    {
        if (companyId > 0)
        {
            await companyRepository.DeleteAsync(companyId);
        }
    }
}
