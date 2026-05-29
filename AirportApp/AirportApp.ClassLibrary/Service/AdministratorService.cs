using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class AdministratorService(IAdministratorRepository administratorRepository) : IAdministratorService
{
    public async Task<Administrator?> GetAdministratorByIdAsync(int identificationNumber)
    {
        return await administratorRepository.GetByIdAsync(identificationNumber);
    }

    public async Task<int> AddAdministratorAsync(Administrator administratorEntity)
    {
        return await administratorRepository.AddAsync(administratorEntity);
    }

    public async Task UpdateAdministratorByIdAsync(int identificationNumber, Administrator administratorEntity)
    {
        await administratorRepository.UpdateAsync(administratorEntity);
    }

    public async Task DeleteAdministratorByIdAsync(int identificationNumber)
    {
        await administratorRepository.DeleteAsync(identificationNumber);
    }

    public async Task<List<Administrator>> GetAllAdministratorsAsync()
    {
        return (await administratorRepository.GetAsync()).ToList();
    }

    public async Task CreateNewAdministratorAsync(int identificationNumber, string fullName, string emailAddress, string departmentName)
    {
        EmployeeDepartment departmentEnum = (EmployeeDepartment)Enum.Parse(typeof(EmployeeDepartment), departmentName);
        Administrator newAdministrator = new Administrator(identificationNumber, fullName, emailAddress, departmentEnum);
        await ValidateAdministratorIntegrityAsync(newAdministrator);
        await AddAdministratorAsync(newAdministrator);
    }

    public async Task ValidateAdministratorIntegrityAsync(Administrator administratorEntity)
    {
        ArgumentNullException.ThrowIfNull(administratorEntity);

        if ((await GetAllAdministratorsAsync()).Contains(administratorEntity))
        {
            throw new ArgumentException("Administrator already exists");
        }
        if (string.IsNullOrEmpty(administratorEntity.RetrieveConfiguredDisplayFullNameForBot()))
        {
            throw new ArgumentException("Name cannot be null or empty");
        }
        if (string.IsNullOrEmpty(administratorEntity.RetrieveConfiguredEmailAddressForBotContact()))
        {
            throw new ArgumentException("Email cannot be null or empty");
        }
        if (!Enum.IsDefined(typeof(EmployeeDepartment), administratorEntity.GetDepartmentName()))
        {
            throw new ArgumentException("Invalid group");
        }
    }
}
