using AirportAPI.Repositories.Interfaces;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportAPI.Repositories
{
    public class EfEmployeeFlightRepository(AppDbContext databaseContext) : IEmployeeFlightRepository
    {
        public void AssignFlightToEmployeeUsingIds(int employeeId, int flightId)
        {
            Employee employee = databaseContext.Employees.Find(employeeId)
                ?? throw new InvalidOperationException($"The employee with Id {employeeId} was not found.");

            Flight flight = databaseContext.Flights.Find(flightId)
                ?? throw new InvalidOperationException($"The flight with Id {flightId} was not found.");

            EmployeeFlight employeeFlightAssignment = new EmployeeFlight
            {
                Employee = employee,
                Flight = flight
            };

            databaseContext.EmployeeFlights.Add(employeeFlightAssignment);
            databaseContext.SaveChanges();
        }

        public void RemoveFlightFromEmployeeUsingIds(int employeeId, int flightId)
        {
            EmployeeFlight? assignmentToRemove = databaseContext.EmployeeFlights
                .FirstOrDefault(assignment =>
                    assignment.Employee.Id == employeeId &&
                    assignment.Flight.Id == flightId);

            if (assignmentToRemove == null)
            {
                return;
            }

            databaseContext.EmployeeFlights.Remove(assignmentToRemove);
            databaseContext.SaveChanges();
        }

        public List<int> GetFlightsByEmployeeId(int employeeId)
        {
            return databaseContext.EmployeeFlights
                .Where(assignment => assignment.Employee.Id == employeeId)
                .Select(assignment => assignment.Flight.Id)
                .ToList();
        }

        public List<int> GetEmployeesByFlightId(int flightId)
        {
            return databaseContext.EmployeeFlights
                .Where(assignment => assignment.Flight.Id == flightId)
                .Select(assignment => assignment.Employee.Id)
                .ToList();
        }

        public void RemoveAllByFlightId(int flightId)
        {
            List<EmployeeFlight> assignmentsForFlight = databaseContext.EmployeeFlights
                .Where(assignment => assignment.Flight.Id == flightId)
                .ToList();

            if (assignmentsForFlight.Count == 0)
            {
                return;
            }

            databaseContext.EmployeeFlights.RemoveRange(assignmentsForFlight);
            databaseContext.SaveChanges();
        }

        public void RemoveAllByEmployeeId(int employeeId)
        {
            List<EmployeeFlight> assignmentsForEmployee = databaseContext.EmployeeFlights
                .Where(assignment => assignment.Employee.Id == employeeId)
                .ToList();

            if (assignmentsForEmployee.Count == 0)
            {
                return;
            }

            databaseContext.EmployeeFlights.RemoveRange(assignmentsForEmployee);
            databaseContext.SaveChanges();
        }
    }
}