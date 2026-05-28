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
}
