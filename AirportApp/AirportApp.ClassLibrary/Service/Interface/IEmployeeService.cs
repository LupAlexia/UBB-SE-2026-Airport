using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(int employeeId);
    Task<IEnumerable<Employee>> GetEmployeesByRoleAsync(EmployeeRoleEnum role);
    Task SaveEmployeeAsync(Employee employee);
    Task DeleteEmployeeAsync(int employeeId);
    Task DeleteWithAssignmentsAsync(int employeeId);
    EmployeeRoleEnum ParseRole(string roleText);
    Task<IEnumerable<Employee>> GetPilotsAsync();
    Task<IEnumerable<Employee>> GetFlightAttendantsAsync();
    Task<IEnumerable<Employee>> GetCoPilotsAsync();
    Task<IEnumerable<Employee>> GetFlightDispatchersAsync();
    Task<int> AddEmployeeAsync(string name, EmployeeRoleEnum role, DateOnly birthday, int salary, DateOnly hiringDate);
    Task UpdateEmployeeAsync(int id, string? name = null, EmployeeRoleEnum? role = null, int? salary = null, DateOnly? birthday = null, DateOnly? hiringDate = null);
    Task DeleteEmployeeUsingIdAsync(int employeeId);
    Task<int> LoginAsync(string employeeIdText);
}
