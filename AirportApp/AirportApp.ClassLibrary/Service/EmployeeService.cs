using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class EmployeeService(
    IEmployeeRepository employeeRepository,
    IEmployeeFlightService employeeFlightService) : IEmployeeService
{
    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await employeeRepository.GetAsync();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
    {
        if (employeeId <= 0)
        {
            return null;
        }

        return await employeeRepository.GetByIdAsync(employeeId);
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

    public async Task<IEnumerable<Employee>> GetEmployeesByRoleAsync(EmployeeRoleEnum role)
    {
        IEnumerable<Employee> allEmployees = await GetAllEmployeesAsync();
        List<Employee> filteredEmployees = new List<Employee>();

        foreach (Employee employee in allEmployees)
        {
            if (employee.Role == role)
            {
                filteredEmployees.Add(employee);
            }
        }

        return filteredEmployees;
    }

    public async Task SaveEmployeeAsync(Employee editingEmployee, DateTimeOffset? birthday, DateTimeOffset? hiringDate, string salaryText)
    {
        if (editingEmployee == null)
        {
            throw new ArgumentException("Invalid employee selected.");
        }

        if (birthday == null || hiringDate == null)
        {
            throw new ArgumentException("Birthday and Hiring Date are required.");
        }

        if (!int.TryParse(salaryText, out int parsedSalary))
        {
            throw new ArgumentException("Salary must be a valid number.");
        }

        DateOnly finalBirthday = DateOnly.FromDateTime(birthday.Value.DateTime);
        DateOnly finalHiringDate = DateOnly.FromDateTime(hiringDate.Value.DateTime);

        if (finalBirthday > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ArgumentException("Birthday cannot be in the future.");
        }

        editingEmployee.Salary = parsedSalary;

        if (editingEmployee.Id == 0)
        {
            await AddEmployeeAsync(
                editingEmployee.Name,
                editingEmployee.Role,
                finalBirthday,
                editingEmployee.Salary,
                finalHiringDate);
        }
        else
        {
            await UpdateEmployeeAsync(
                editingEmployee.Id,
                editingEmployee.Name,
                editingEmployee.Role,
                editingEmployee.Salary,
                finalBirthday,
                finalHiringDate);
        }
    }

    public async Task SaveEmployeeAsync(Employee employee)
    {
        await employeeRepository.AddAsync(employee);
    }

    public async Task DeleteWithAssignmentsAsync(int employeeId)
    {
        if (employeeId <= 0)
        {
            throw new ArgumentException("Invalid employee selected.");
        }

        await employeeFlightService.RemoveAllFlightsAssignmentsForEmployeeAsync(employeeId);
        await DeleteEmployeeUsingIdAsync(employeeId);
    }

    public async Task DeleteEmployeeAsync(int employeeId)
    {
        await DeleteEmployeeUsingIdAsync(employeeId);
    }

    public async Task<int> AddEmployeeAsync(string name, EmployeeRoleEnum role, DateOnly birthday, int salary, DateOnly hiringDate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name can not be empty.");
        }

        if (salary < 0)
        {
            throw new ArgumentException("Salary can not be negative.");
        }

        if (birthday > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ArgumentException("Birthday cannot be in the future.");
        }

        if (hiringDate > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ArgumentException("Hiring date can not be in the future.");
        }

        Employee newEmployee = new Employee
        {
            Name = name,
            Role = role,
            Birthday = birthday,
            Salary = salary,
            HiringDate = hiringDate
        };

        return await employeeRepository.AddAsync(newEmployee);
    }

    public async Task UpdateEmployeeAsync(
        int employeeId,
        string? name = null,
        EmployeeRoleEnum? role = null,
        int? salary = null,
        DateOnly? birthday = null,
        DateOnly? hiringDate = null)
    {
        Employee? existingEmployee = await employeeRepository.GetByIdAsync(employeeId);

        if (existingEmployee == null)
        {
            return;
        }

        if (name != null)
        {
            existingEmployee.Name = name;
        }

        if (role.HasValue)
        {
            existingEmployee.Role = role.Value;
        }

        if (salary.HasValue)
        {
            existingEmployee.Salary = salary.Value;
        }

        if (birthday.HasValue)
        {
            existingEmployee.Birthday = birthday.Value;
        }

        if (hiringDate.HasValue)
        {
            existingEmployee.HiringDate = hiringDate.Value;
        }

        await employeeRepository.UpdateAsync(existingEmployee);
    }

    public async Task DeleteEmployeeUsingIdAsync(int employeeId)
    {
        if (employeeId <= 0)
        {
            return;
        }

        await employeeRepository.DeleteAsync(employeeId);
    }

    public EmployeeRoleEnum ParseRole(string roleText)
    {
        if (string.IsNullOrWhiteSpace(roleText))
        {
            return EmployeeRoleEnum.Other;
        }

        string normalized = roleText
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        return Enum.TryParse(normalized, ignoreCase: true, out EmployeeRoleEnum result)
            ? result
            : EmployeeRoleEnum.Other;
    }

    public async Task<int> LoginAsync(string employeeIdText)
    {
        if (!int.TryParse(employeeIdText, out int employeeId))
        {
            throw new ArgumentException("Invalid employee ID format.");
        }

        Employee? employee = await GetEmployeeByIdAsync(employeeId);
        if (employee == null)
        {
            throw new ArgumentException("Employee not found.");
        }

        return employee.Id;
    }
}
