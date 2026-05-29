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

    public async Task<IEnumerable<Employee>> GetPilotsAsync()
    {
        return await GetEmployeesByRoleAsync(EmployeeRoleEnum.Pilot);
    }

    public async Task<IEnumerable<Employee>> GetFlightAttendantsAsync()
    {
        return await GetEmployeesByRoleAsync(EmployeeRoleEnum.FlightAttendant);
    }

    public async Task<IEnumerable<Employee>> GetCoPilotsAsync()
    {
        return await GetEmployeesByRoleAsync(EmployeeRoleEnum.CoPilot);
    }

    public async Task<IEnumerable<Employee>> GetFlightDispatchersAsync()
    {
        return await GetEmployeesByRoleAsync(EmployeeRoleEnum.FlightDispatcher);
    }

    public async Task<int> AddEmployeeAsync(string name, EmployeeRoleEnum role, DateOnly birthday, int salary, DateOnly hiringDate)
    {
        var employee = new Employee
        {
            Name = name,
            Role = role,
            Birthday = birthday,
            Salary = salary,
            HiringDate = hiringDate
        };
        return await employeeRepository.AddAsync(employee);
    }

    public async Task UpdateEmployeeAsync(int id, string? name = null, EmployeeRoleEnum? role = null, int? salary = null, DateOnly? birthday = null, DateOnly? hiringDate = null)
    {
        var employee = await employeeRepository.GetByIdAsync(id);
        if (employee is null)
            throw new InvalidOperationException($"Employee with ID {id} not found.");

        if (name is not null) employee.Name = name;
        if (role.HasValue) employee.Role = role.Value;
        if (salary.HasValue) employee.Salary = salary.Value;
        if (birthday.HasValue) employee.Birthday = birthday.Value;
        if (hiringDate.HasValue) employee.HiringDate = hiringDate.Value;

        await employeeRepository.UpdateAsync(employee);
    }

    public async Task DeleteEmployeeUsingIdAsync(int employeeId)
    {
        await employeeRepository.DeleteAsync(employeeId);
    }

    public async Task<int> LoginAsync(string employeeIdText)
    {
        if (!int.TryParse(employeeIdText, out int employeeId))
            return -1;
        var employee = await employeeRepository.GetByIdAsync(employeeId);
        return employee is not null ? employee.Id : -1;
    }
}
