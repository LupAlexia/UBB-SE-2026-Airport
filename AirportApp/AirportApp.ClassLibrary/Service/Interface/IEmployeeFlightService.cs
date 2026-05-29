using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IEmployeeFlightService
{
    Task AssignEmployeeToFlightUsingIdsAsync(int flightId, int employeeId);
    Task RemoveEmployeeFromFlightUsingIdsAsync(int flightId, int employeeId);
    Task RemoveAllCrewAssignmentsForFlightAsync(int flightId);
    Task RemoveAllFlightsAssignmentsForEmployeeAsync(int employeeId);
    Task<IEnumerable<Employee>> GetEmployeesAssignedToFlightAsync(int flightId);
    Task<IEnumerable<Flight>> GetEmployeeScheduleAsync(int employeeId);
    Task<bool> IsEmployeeAvailableAsync(int employeeId, DateTime targetDate, int targetRouteId, int? excludedFlightId = null);
    Task<IEnumerable<EmployeeScheduleItem>> GetFormattedEmployeeScheduleAsync(int employeeId);
    Task<IEnumerable<CrewMemberSelectionData>> GetCrewSelectionDataAsync(int flightId);
    Task<IEnumerable<Employee>> GetAvailableEmployeesGroupedByRoleAsync(int flightId);
    Task UpdateEmployeesForFlightUsingIdsAsync(int flightId, IEnumerable<int> newEmployeeIds);
    Task AssignEmployeesToFlightUsingIdsAsync(int flightId, IEnumerable<int> employeeIds);
    string FormatCrewListByFlightId(int flightId);
}
