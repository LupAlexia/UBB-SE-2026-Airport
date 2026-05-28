namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IEmployeeFlightRepository
{
    Task AssignAsync(int employeeId, int flightId);
    Task UnassignAsync(int employeeId, int flightId);
    Task<IEnumerable<int>> GetFlightIdsByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<int>> GetEmployeeIdsByFlightIdAsync(int flightId);
    Task DeleteByFlightIdAsync(int flightId);
    Task DeleteByEmployeeIdAsync(int employeeId);
}