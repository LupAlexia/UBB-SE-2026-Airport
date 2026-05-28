using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class EmployeeRepository(AppDbContext databaseContext) : IEmployeeRepository
{
    public async Task<IEnumerable<Employee>> GetAsync()
    {
        return await databaseContext.Employees
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int employeeId)
    {
        return await databaseContext.Employees.FindAsync(employeeId);
    }

    public async Task<int> AddAsync(Employee employee)
    {
        if (employee is null)
        {
            throw new ArgumentNullException(nameof(employee));
        }

        employee.Id = 0;
        databaseContext.Employees.Add(employee);
        await databaseContext.SaveChangesAsync();

        return employee.Id;
    }

    public async Task UpdateAsync(Employee employee)
    {
        if (employee is null)
        {
            throw new ArgumentNullException(nameof(employee));
        }

        var existingEmployee = await databaseContext.Employees.FindAsync(employee.Id);
        if (existingEmployee is null)
        {
            return;
        }

        existingEmployee.Name = employee.Name;
        existingEmployee.Role = employee.Role;
        existingEmployee.Birthday = employee.Birthday;
        existingEmployee.Salary = employee.Salary;
        existingEmployee.HiringDate = employee.HiringDate;

        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int employeeId)
    {
        var employeeToRemove = await databaseContext.Employees.FindAsync(employeeId);
        if (employeeToRemove is null)
        {
            return;
        }

        databaseContext.Employees.Remove(employeeToRemove);
        await databaseContext.SaveChangesAsync();
    }
}