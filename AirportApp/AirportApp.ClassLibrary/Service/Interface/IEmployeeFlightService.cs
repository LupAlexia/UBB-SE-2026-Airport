using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IEmployeeFlightService
{
    Task AssignEmployeeToFlightUsingIdsAsync(int flightId, int employeeId);
    Task RemoveEmployeeFromFlightUsingIdsAsync(int flightId, int employeeId);
    Task<IEnumerable<Employee>> GetEmployeesAssignedToFlightAsync(int flightId);
    Task<IEnumerable<Flight>> GetEmployeeScheduleAsync(int employeeId);
    Task<bool> IsEmployeeAvailableAsync(int employeeId, DateTime targetDate, int targetRouteId, int? excludedFlightId = null);
    Task AssignEmpolyeesToFlightUsingIdsAsync(int flightId, List<int> employeeIds);
    Task UpdateEmployeesForFlightUsingIdsAsync(int flightId, List<int> updatedEmployeeIds);
    Task RemoveAllCrewAssignmentsForFlightAsync(int flightId);
    Task RemoveAllFlightsAssignmentsForEmployeeAsync(int employeeId);
    Task<IEnumerable<EmployeeScheduleItem>> GetFormattedEmployeeScheduleAsync(int employeeId);
    Task<IEnumerable<Employee>> GetAvailableEmployeesGroupedByRoleAsync(Flight flight);
    Task<IEnumerable<Employee>> GetAvailableEmployeesGroupedByRoleByIdAsync(int flightId);
    string FormatCrewList(int flightId);
    Task<IEnumerable<CrewMemberSelectionData>> GetCrewSelectionDataAsync(Flight flight);
    Task<IEnumerable<CrewMemberSelectionData>> GetCrewSelectionDataByIdAsync(int flightId);
}
