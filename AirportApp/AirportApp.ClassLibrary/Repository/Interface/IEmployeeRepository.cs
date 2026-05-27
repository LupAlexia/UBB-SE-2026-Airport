using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAsync();
    Task<Employee?> GetByIdAsync(int employeeId);
    Task<int> AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(int employeeId);
}