using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IAdministratorRepository
{
    Task<IEnumerable<Administrator>> GetAsync();
    Task<Administrator?> GetByIdAsync(int administratorId);
    Task<int> AddAsync(Administrator administrator);
    Task UpdateAsync(Administrator administrator);
    Task DeleteAsync(int administratorId);
}