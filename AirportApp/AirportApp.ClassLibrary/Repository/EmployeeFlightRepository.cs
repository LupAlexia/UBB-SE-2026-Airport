using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class EmployeeFlightRepository(AppDbContext databaseContext) : IEmployeeFlightRepository
{
    public async Task AssignAsync(int employeeId, int flightId)
    {
        var employee = await databaseContext.Employees.FindAsync(employeeId)
            ?? throw new InvalidOperationException($"Employee with Id {employeeId} was not found.");

        var flight = await databaseContext.Flights.FindAsync(flightId)
            ?? throw new InvalidOperationException($"Flight with Id {flightId} was not found.");

        var employeeFlightAssignment = new EmployeeFlight
        {
            Employee = employee,
            Flight = flight
        };

        databaseContext.EmployeeFlights.Add(employeeFlightAssignment);
        await databaseContext.SaveChangesAsync();
    }

    public async Task UnassignAsync(int employeeId, int flightId)
    {
        var assignmentToRemove = await databaseContext.EmployeeFlights
            .FirstOrDefaultAsync(assignment =>
                assignment.Employee.Id == employeeId &&
                assignment.Flight.Id == flightId);

        if (assignmentToRemove is null)
        {
            return;
        }

        databaseContext.EmployeeFlights.Remove(assignmentToRemove);
        await databaseContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<int>> GetFlightIdsByEmployeeIdAsync(int employeeId)
    {
        return await databaseContext.EmployeeFlights
            .Where(assignment => assignment.Employee.Id == employeeId)
            .Select(assignment => assignment.Flight.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<int>> GetEmployeeIdsByFlightIdAsync(int flightId)
    {
        return await databaseContext.EmployeeFlights
            .Where(assignment => assignment.Flight.Id == flightId)
            .Select(assignment => assignment.Employee.Id)
            .ToListAsync();
    }

    public async Task DeleteByFlightIdAsync(int flightId)
    {
        var assignmentsForFlight = await databaseContext.EmployeeFlights
            .Where(assignment => assignment.Flight.Id == flightId)
            .ToListAsync();

        if (assignmentsForFlight.Count == 0)
        {
            return;
        }

        databaseContext.EmployeeFlights.RemoveRange(assignmentsForFlight);
        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteByEmployeeIdAsync(int employeeId)
    {
        var assignmentsForEmployee = await databaseContext.EmployeeFlights
            .Where(assignment => assignment.Employee.Id == employeeId)
            .ToListAsync();

        if (assignmentsForEmployee.Count == 0)
        {
            return;
        }

        databaseContext.EmployeeFlights.RemoveRange(assignmentsForEmployee);
        await databaseContext.SaveChangesAsync();
    }
}