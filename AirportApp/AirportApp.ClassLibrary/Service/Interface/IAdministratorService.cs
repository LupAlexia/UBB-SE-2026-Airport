using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IAdministratorService
{
    Task<IEnumerable<Administrator>> GetAllAsync();
    Task<Administrator?> GetByIdAsync(int administratorId);
    Task AddAsync(Administrator administrator);
    Task UpdateAsync(Administrator administrator);
    Task DeleteAsync(int administratorId);
}
