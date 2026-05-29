using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IAdministratorService
{
    Task<Administrator?> GetAdministratorByIdAsync(int identificationNumber);
    Task<int> AddAdministratorAsync(Administrator administratorEntity);
    Task UpdateAdministratorByIdAsync(int identificationNumber, Administrator administratorEntity);
    Task DeleteAdministratorByIdAsync(int identificationNumber);
    Task<List<Administrator>> GetAllAdministratorsAsync();
    Task CreateNewAdministratorAsync(int identificationNumber, string fullName, string emailAddress, string departmentName);
    Task ValidateAdministratorIntegrityAsync(Administrator administratorEntity);
}
