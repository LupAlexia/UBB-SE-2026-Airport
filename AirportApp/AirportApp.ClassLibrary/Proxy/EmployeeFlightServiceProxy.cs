using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class EmployeeFlightServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IEmployeeFlightService
{
    private const string BaseUrl = "api/employee-flights";

    public async Task AssignEmployeeToFlightUsingIdsAsync(int flightId, int employeeId)
    {
        var dto = new AssignEmployeeDto { FlightId = flightId, EmployeeId = employeeId };
        await PostAsync($"{BaseUrl}/assign", dto);
    }

    public async Task RemoveEmployeeFromFlightUsingIdsAsync(int flightId, int employeeId)
    {
        await DeleteAsync($"{BaseUrl}/flights/{flightId}/employees/{employeeId}");
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAssignedToFlightAsync(int flightId)
    {
        return await GetListAsync<Employee>($"{BaseUrl}/flights/{flightId}/employees");
    }

    public async Task<IEnumerable<Flight>> GetEmployeeScheduleAsync(int employeeId)
    {
        return await GetListAsync<Flight>($"{BaseUrl}/employees/{employeeId}/schedule");
    }

    public async Task<bool> IsEmployeeAvailableAsync(int employeeId, DateTime targetDate, int targetRouteId, int? excludedFlightId = null)
    {
        return await GetRequiredAsync<bool>($"{BaseUrl}/employees/{employeeId}/available?targetDate={targetDate:o}&targetRouteId={targetRouteId}&excludedFlightId={excludedFlightId}");
    }

    public async Task AssignEmpolyeesToFlightUsingIdsAsync(int flightId, List<int> employeeIds)
    {
        await PostAsync($"{BaseUrl}/flights/{flightId}/employees", employeeIds);
    }

    public async Task UpdateEmployeesForFlightUsingIdsAsync(int flightId, List<int> updatedEmployeeIds)
    {
        await PutAsync($"{BaseUrl}/flights/{flightId}/employees", updatedEmployeeIds);
    }

    public async Task RemoveAllCrewAssignmentsForFlightAsync(int flightId)
    {
        await DeleteAsync($"{BaseUrl}/flights/{flightId}");
    }

    public async Task RemoveAllFlightsAssignmentsForEmployeeAsync(int employeeId)
    {
        await DeleteAsync($"{BaseUrl}/employees/{employeeId}");
    }

    public async Task<IEnumerable<EmployeeScheduleItem>> GetFormattedEmployeeScheduleAsync(int employeeId)
    {
        return await GetListAsync<EmployeeScheduleItem>($"{BaseUrl}/employees/{employeeId}/formatted-schedule");
    }

    public async Task<IEnumerable<Employee>> GetAvailableEmployeesGroupedByRoleAsync(Flight flight)
    {
        return await GetListAsync<Employee>($"{BaseUrl}/flights/{flight.Id}/available-employees");
    }

    public async Task<IEnumerable<Employee>> GetAvailableEmployeesGroupedByRoleByIdAsync(int flightId)
    {
        return await GetListAsync<Employee>($"{BaseUrl}/flights/{flightId}/available-employees");
    }

    public string FormatCrewList(int flightId)
    {
        var response = GetRequiredAsync<CrewListResponse>($"{BaseUrl}/flights/{flightId}/crew-list").GetAwaiter().GetResult();
        return response.Result ?? string.Empty;
    }

    public async Task<IEnumerable<CrewMemberSelectionData>> GetCrewSelectionDataAsync(Flight flight)
    {
        return await GetListAsync<CrewMemberSelectionData>($"{BaseUrl}/flights/{flight.Id}/crew-selection-data");
    }

    public async Task<IEnumerable<CrewMemberSelectionData>> GetCrewSelectionDataByIdAsync(int flightId)
    {
        return await GetListAsync<CrewMemberSelectionData>($"{BaseUrl}/flights/{flightId}/crew-selection-data");
    }

    private sealed class CrewListResponse
    {
        public string? Result { get; set; }
    }
}
