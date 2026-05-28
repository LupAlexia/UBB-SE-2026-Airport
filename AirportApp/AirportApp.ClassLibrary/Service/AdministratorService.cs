using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class AdministratorService(IAdministratorRepository administratorRepository) : IAdministratorService
{
    public async Task<IEnumerable<Administrator>> GetAllAsync()
    {
        return await administratorRepository.GetAsync();
    }

    public async Task<Administrator?> GetByIdAsync(int administratorId)
    {
        return await administratorRepository.GetByIdAsync(administratorId);
    }

    public async Task AddAsync(Administrator administrator)
    {
        await administratorRepository.AddAsync(administrator);
    }

    public async Task UpdateAsync(Administrator administrator)
    {
        await administratorRepository.UpdateAsync(administrator);
    }

    public async Task DeleteAsync(int administratorId)
    {
        await administratorRepository.DeleteAsync(administratorId);
    }
}
