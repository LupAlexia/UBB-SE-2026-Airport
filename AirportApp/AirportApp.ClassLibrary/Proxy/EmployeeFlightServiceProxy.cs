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
    private const string BaseUrl = "api/employeeflights";

    public async Task AssignEmployeeToFlightUsingIdsAsync(int flightId, int employeeId)
    {
        var dto = new AssignEmployeeDto { FlightId = flightId, EmployeeId = employeeId };
        await PostAsync($"{BaseUrl}/assign", dto);
    }

    public async Task RemoveEmployeeFromFlightUsingIdsAsync(int flightId, int employeeId)
    {
        await DeleteAsync($"{BaseUrl}/remove?flightId={flightId}&employeeId={employeeId}");
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAssignedToFlightAsync(int flightId)
    {
        return await GetListAsync<Employee>($"{BaseUrl}/flight/{flightId}/employees");
    }

    public async Task<IEnumerable<Flight>> GetEmployeeScheduleAsync(int employeeId)
    {
        return await GetListAsync<Flight>($"{BaseUrl}/employee/{employeeId}/schedule");
    }

    public async Task<bool> IsEmployeeAvailableAsync(int employeeId, DateTime targetDate, int targetRouteId, int? excludedFlightId = null)
    {
        return await GetRequiredAsync<bool>($"{BaseUrl}/employee/{employeeId}/available?targetDate={targetDate:o}&targetRouteId={targetRouteId}&excludedFlightId={excludedFlightId}");
    }

    public async Task AssignEmpolyeesToFlightUsingIdsAsync(int flightId, List<int> employeeIds)
    {
        await PostAsync($"{BaseUrl}/assign-multiple?flightId={flightId}", employeeIds);
    }

    public async Task UpdateEmployeesForFlightUsingIdsAsync(int flightId, List<int> updatedEmployeeIds)
    {
        await PutAsync($"{BaseUrl}/flight/{flightId}/employees", updatedEmployeeIds);
    }

    public async Task RemoveAllCrewAssignmentsForFlightAsync(int flightId)
    {
        await DeleteAsync($"{BaseUrl}/flight/{flightId}/crew");
    }

    public async Task RemoveAllFlightsAssignmentsForEmployeeAsync(int employeeId)
    {
        await DeleteAsync($"{BaseUrl}/employee/{employeeId}/flights");
    }

    public async Task<IEnumerable<EmployeeScheduleItem>> GetFormattedEmployeeScheduleAsync(int employeeId)
    {
        return await GetListAsync<EmployeeScheduleItem>($"{BaseUrl}/employee/{employeeId}/schedule-formatted");
    }

    public async Task<IEnumerable<Employee>> GetAvailableEmployeesGroupedByRoleAsync(Flight flight)
    {
        return await PostForResultAsync<Flight, IEnumerable<Employee>>($"{BaseUrl}/available-employees", flight);
    }

    public async Task<IEnumerable<Employee>> GetAvailableEmployeesGroupedByRoleByIdAsync(int flightId)
    {
        return await GetListAsync<Employee>($"{BaseUrl}/available-employees-by-id/{flightId}");
    }

    public string FormatCrewList(int flightId)
    {
        return GetRequiredAsync<string>($"{BaseUrl}/flight/{flightId}/crew-formatted").GetAwaiter().GetResult();
    }

    public async Task<IEnumerable<CrewMemberSelectionData>> GetCrewSelectionDataAsync(Flight flight)
    {
        return await PostForResultAsync<Flight, IEnumerable<CrewMemberSelectionData>>($"{BaseUrl}/crew-selection-data", flight);
    }

    public async Task<IEnumerable<CrewMemberSelectionData>> GetCrewSelectionDataByIdAsync(int flightId)
    {
        return await GetListAsync<CrewMemberSelectionData>($"{BaseUrl}/flight/{flightId}/crew-selection-data");
    }
}
