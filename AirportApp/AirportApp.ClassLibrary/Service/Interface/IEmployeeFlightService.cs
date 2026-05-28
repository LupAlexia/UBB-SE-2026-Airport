using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IEmployeeFlightService
{
    Task AssignEmployeeToFlightUsingIdsAsync(int employeeId, int flightId);
    Task UnassignEmployeeFromFlightAsync(int employeeId, int flightId);
    Task<IEnumerable<int>> GetFlightIdsByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<int>> GetEmployeeIdsByFlightIdAsync(int flightId);
    Task<bool> IsEmployeeAvailableAsync(int employeeId, int flightId);
    Task<IEnumerable<EmployeeScheduleItem>> GetFormattedEmployeeScheduleAsync(int employeeId);
    Task<IEnumerable<CrewMemberSelectionData>> GetCrewSelectionDataAsync(int flightId);
    Task<IEnumerable<Employee>> GetAvailableEmployeesGroupedByRoleAsync(int flightId);
    Task UpdateEmployeesForFlightUsingIdsAsync(int flightId, IEnumerable<int> newEmployeeIds);
    Task AssignEmployeesToFlightUsingIdsAsync(int flightId, IEnumerable<int> employeeIds);
    string FormatCrewList(IEnumerable<string> names);
}
