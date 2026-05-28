using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class EmployeeFlightService(
    IEmployeeFlightRepository employeeFlightRepository,
    IEmployeeRepository employeeRepository,
    IFlightRepository flightRepository,
    IRouteRepository routeRepository,
    IGateService gateService,
    IRunwayService runwayService) : IEmployeeFlightService
{
    public async Task AssignEmployeeToFlightUsingIdsAsync(int employeeId, int flightId)
    {
        if (employeeId <= 0) throw new ArgumentException("Invalid employee ID.");
        if (flightId <= 0) throw new ArgumentException("Invalid flight ID.");

        var employee = await employeeRepository.GetByIdAsync(employeeId);
        if (employee is null) throw new InvalidOperationException($"Employee with ID {employeeId} not found.");

        var flight = await flightRepository.GetByIdAsync(flightId);
        if (flight is null) throw new InvalidOperationException($"Flight with ID {flightId} not found.");

        var assigned = await employeeFlightRepository.GetEmployeeIdsByFlightIdAsync(flightId);
        if (assigned.Contains(employeeId))
            throw new InvalidOperationException($"Employee {employeeId} is already assigned to flight {flightId}.");

        bool available = await IsEmployeeAvailableAsync(employeeId, flightId);
        if (!available)
            throw new InvalidOperationException($"Employee {employeeId} is not available for flight {flightId} due to a schedule conflict.");

        await employeeFlightRepository.AssignAsync(employeeId, flightId);
    }

    public async Task UnassignEmployeeFromFlightAsync(int employeeId, int flightId)
    {
        await employeeFlightRepository.UnassignAsync(employeeId, flightId);
    }

    public async Task<IEnumerable<int>> GetFlightIdsByEmployeeIdAsync(int employeeId)
    {
        return await employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(employeeId);
    }

    public async Task<IEnumerable<int>> GetEmployeeIdsByFlightIdAsync(int flightId)
    {
        return await employeeFlightRepository.GetEmployeeIdsByFlightIdAsync(flightId);
    }

    public async Task<bool> IsEmployeeAvailableAsync(int employeeId, int targetFlightId)
    {
        var targetFlight = await flightRepository.GetByIdAsync(targetFlightId);
        if (targetFlight is null) return false;

        var targetRoute = targetFlight.Route is not null
            ? await routeRepository.GetByIdAsync(targetFlight.Route.Id)
            : null;
        if (targetRoute is null) return true;

        int targetStart = targetRoute.DepartureTime.Hour * 60 + targetRoute.DepartureTime.Minute;
        int targetEnd = targetRoute.ArrivalTime.Hour * 60 + targetRoute.ArrivalTime.Minute;
        if (targetEnd <= targetStart) targetEnd += 1440;

        var assignedFlightIds = await employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(employeeId);
        foreach (int assignedFlightId in assignedFlightIds)
        {
            var assignedFlight = await flightRepository.GetByIdAsync(assignedFlightId);
            if (assignedFlight is null) continue;
            if (assignedFlight.Date.Date != targetFlight.Date.Date) continue;

            var assignedRoute = assignedFlight.Route is not null
                ? await routeRepository.GetByIdAsync(assignedFlight.Route.Id)
                : null;
            if (assignedRoute is null) continue;

            int start = assignedRoute.DepartureTime.Hour * 60 + assignedRoute.DepartureTime.Minute;
            int end = assignedRoute.ArrivalTime.Hour * 60 + assignedRoute.ArrivalTime.Minute;
            if (end <= start) end += 1440;

            if (targetStart < end && start < targetEnd)
                return false;
        }

        return true;
    }

    public async Task<IEnumerable<EmployeeScheduleItem>> GetFormattedEmployeeScheduleAsync(int employeeId)
    {
        var flightIds = await employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(employeeId);
        var scheduleItems = new List<EmployeeScheduleItem>();

        foreach (int flightId in flightIds)
        {
            var flight = await flightRepository.GetByIdAsync(flightId);
            if (flight is null) continue;

            var route = flight.Route is not null
                ? await routeRepository.GetByIdAsync(flight.Route.Id)
                : null;

            string flightType = route?.RouteType ?? "-";
            string flightTime = route is null ? "-"
                : flightType == "ARR"
                    ? route.ArrivalTime.ToString("HH:mm")
                    : route.DepartureTime.ToString("HH:mm");

            var gate = flight.Gate is not null
                ? await gateService.GetGateByIdAsync(flight.Gate.Id)
                : null;

            var runway = flight.Runway is not null
                ? await runwayService.GetRunwayByIdAsync(flight.Runway.Id)
                : null;

            scheduleItems.Add(new EmployeeScheduleItem
            {
                Id = flightId.ToString(),
                FlightNumber = flight.FlightNumber,
                FlightType = flightType,
                Date = flight.Date.ToString("yyyy-MM-dd"),
                GateName = gate?.GateName ?? "-",
                RunwayName = runway?.Name ?? "-",
                FlightTime = flightTime
            });
        }

        return scheduleItems;
    }

    public async Task<IEnumerable<CrewMemberSelectionData>> GetCrewSelectionDataAsync(int flightId)
    {
        var availableEmployees = await GetAvailableEmployeesGroupedByRoleAsync(flightId);
        var sorted = availableEmployees
            .OrderBy(e => (int)e.Role)
            .ThenBy(e => e.Name)
            .ToList();

        var result = new List<CrewMemberSelectionData>();
        EmployeeRoleEnum? lastRole = null;

        foreach (var employee in sorted)
        {
            bool isFirst = employee.Role != lastRole;
            result.Add(new CrewMemberSelectionData
            {
                Employee = employee,
                IsSelected = false,
                IsFirstInRoleGroup = isFirst,
                RoleHeader = isFirst ? employee.Role.ToString() : string.Empty
            });
            lastRole = employee.Role;
        }

        return result;
    }

    public async Task<IEnumerable<Employee>> GetAvailableEmployeesGroupedByRoleAsync(int flightId)
    {
        var allEmployees = await employeeRepository.GetAsync();
        var available = new List<Employee>();
        foreach (var employee in allEmployees)
        {
            bool isAvailable = await IsEmployeeAvailableAsync(employee.Id, flightId);
            if (isAvailable)
                available.Add(employee);
        }
        return available;
    }

    public string FormatCrewList(IEnumerable<string> names)
    {
        var nameList = names.ToList();
        if (nameList.Count == 0) return "Unassigned";
        return string.Join(", ", nameList);
    }

    public async Task UpdateEmployeesForFlightUsingIdsAsync(int flightId, IEnumerable<int> newEmployeeIds)
    {
        var currentIds = (await employeeFlightRepository.GetEmployeeIdsByFlightIdAsync(flightId)).ToList();
        var newIdsList = newEmployeeIds.ToList();

        var toRemove = currentIds.Except(newIdsList).ToList();
        var toAdd = newIdsList.Except(currentIds).ToList();

        foreach (int empId in toRemove)
            await employeeFlightRepository.UnassignAsync(empId, flightId);

        foreach (int empId in toAdd)
            await employeeFlightRepository.AssignAsync(empId, flightId);
    }

    public async Task AssignEmployeesToFlightUsingIdsAsync(int flightId, IEnumerable<int> employeeIds)
    {
        foreach (int empId in employeeIds)
        {
            try
            {
                await AssignEmployeeToFlightUsingIdsAsync(empId, flightId);
            }
            catch
            {
            }
        }
    }
}
