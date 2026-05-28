using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class EmployeeService(IEmployeeRepository employeeRepository, IEmployeeFlightRepository employeeFlightRepository) : IEmployeeService
{
    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await employeeRepository.GetAsync();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
    {
        return await employeeRepository.GetByIdAsync(employeeId);
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByRoleAsync(EmployeeRoleEnum role)
    {
        var all = await employeeRepository.GetAsync();
        return all.Where(e => e.Role == role).ToList();
    }

    public async Task SaveEmployeeAsync(Employee employee)
    {
        if (string.IsNullOrWhiteSpace(employee.Name))
            throw new ArgumentException("Employee name cannot be empty.");

        if (employee.Id == 0)
            await employeeRepository.AddAsync(employee);
        else
            await employeeRepository.UpdateAsync(employee);
    }

    public async Task DeleteEmployeeAsync(int employeeId)
    {
        await employeeRepository.DeleteAsync(employeeId);
    }

    public async Task DeleteWithAssignmentsAsync(int employeeId)
    {
        await employeeFlightRepository.DeleteByEmployeeIdAsync(employeeId);
        await employeeRepository.DeleteAsync(employeeId);
    }

    public EmployeeRoleEnum ParseRole(string roleText)
    {
        if (Enum.TryParse<EmployeeRoleEnum>(roleText, true, out var role))
            return role;
        return EmployeeRoleEnum.Other;
    }
}
