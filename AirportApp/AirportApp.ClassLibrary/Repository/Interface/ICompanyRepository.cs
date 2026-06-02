using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetAsync();
    Task<Company?> GetByIdAsync(int companyId);
    Task<IEnumerable<Company>> GetAllByManagerIdAsync(int managerId);
    Task<int> AddAsync(Company company);
    Task UpdateAsync(Company company);
    Task DeleteAsync(int companyId);
}