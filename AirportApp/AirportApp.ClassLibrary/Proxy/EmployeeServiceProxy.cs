using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class EmployeeServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IEmployeeService
{
    private const string BaseUrl = "api/employees";

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await GetListAsync<Employee>(BaseUrl);
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
    {
        return await GetOptionalAsync<Employee>($"{BaseUrl}/{employeeId}");
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByRoleAsync(EmployeeRoleEnum role)
    {
        return await GetListAsync<Employee>($"{BaseUrl}/role/{role}");
    }

    public async Task SaveEmployeeAsync(Employee employee, DateTimeOffset? birthday, DateTimeOffset? hiringDate, string salaryText)
    {
        var request = new { Employee = employee, Birthday = birthday, HiringDate = hiringDate, SalaryText = salaryText };
        await PostAsync($"{BaseUrl}/save", request);
    }

    public async Task SaveEmployeeAsync(Employee employee)
    {
        await PostAsync($"{BaseUrl}/save-simple", employee);
    }

    public async Task DeleteEmployeeAsync(int employeeId)
    {
        await DeleteAsync($"{BaseUrl}/{employeeId}");
    }

    public async Task DeleteWithAssignmentsAsync(int employeeId)
    {
        await DeleteAsync($"{BaseUrl}/{employeeId}/with-assignments");
    }

    public EmployeeRoleEnum ParseRole(string roleText)
    {
        if (string.IsNullOrWhiteSpace(roleText)) return EmployeeRoleEnum.Other;
        string normalized = roleText.Replace(" ", "").Replace("-", "");
        return Enum.TryParse(normalized, true, out EmployeeRoleEnum result) ? result : EmployeeRoleEnum.Other;
    }

    public async Task<IEnumerable<Employee>> GetPilotsAsync()
    {
        return await GetListAsync<Employee>($"{BaseUrl}/pilots");
    }

    public async Task<IEnumerable<Employee>> GetFlightAttendantsAsync()
    {
        return await GetListAsync<Employee>($"{BaseUrl}/flight-attendants");
    }

    public async Task<IEnumerable<Employee>> GetCoPilotsAsync()
    {
        return await GetListAsync<Employee>($"{BaseUrl}/co-pilots");
    }

    public async Task<IEnumerable<Employee>> GetFlightDispatchersAsync()
    {
        return await GetListAsync<Employee>($"{BaseUrl}/flight-dispatchers");
    }

    public async Task<int> AddEmployeeAsync(string name, EmployeeRoleEnum role, DateOnly birthday, int salary, DateOnly hiringDate)
    {
        var employee = new Employee { Name = name, Role = role, Birthday = birthday, Salary = salary, HiringDate = hiringDate };
        return await PostForResultAsync<Employee, int>(BaseUrl, employee);
    }

    public async Task UpdateEmployeeAsync(int id, string? name = null, EmployeeRoleEnum? role = null, int? salary = null, DateOnly? birthday = null, DateOnly? hiringDate = null)
    {
        var employee = new Employee
        {
            Id = id,
            Name = name ?? "",
            Role = role ?? EmployeeRoleEnum.Other,
            Salary = salary ?? 0,
            Birthday = birthday ?? DateOnly.MinValue,
            HiringDate = hiringDate ?? DateOnly.MinValue
        };
        await PutAsync($"{BaseUrl}/{id}", employee);
    }

    public async Task DeleteEmployeeUsingIdAsync(int employeeId)
    {
        await DeleteAsync($"{BaseUrl}/{employeeId}");
    }

    public async Task<int> LoginAsync(string employeeIdText)
    {
        return await GetRequiredAsync<int>($"{BaseUrl}/login?employeeIdText={Uri.EscapeDataString(employeeIdText)}");
    }
}
