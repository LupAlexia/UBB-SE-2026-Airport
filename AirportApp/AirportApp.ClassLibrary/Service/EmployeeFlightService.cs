using System.Text;
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
    IRunwayService runwayService,
    IRouteService routeService) : IEmployeeFlightService
{
    private const string UnnasignedCrew = "Unassigned";
    private const string FlightDateFormat = "dd MMM yyyy";
    private const string EmptyFieldPlaceholder = "-";

    public async Task AssignEmployeeToFlightUsingIdsAsync(int flightId, int employeeId)
    {
        if (flightId <= 0 || employeeId <= 0)
        {
            throw new ArgumentException("Invalid flight or employee ID.");
        }

        Employee? employee = await employeeRepository.GetByIdAsync(employeeId);
        Flight? flight = await flightRepository.GetByIdAsync(flightId);

        if (employee == null || flight == null)
        {
            throw new InvalidOperationException("Employee or Flight does not exist.");
        }

        IEnumerable<int> currentCrewIds = await employeeFlightRepository.GetEmployeeIdsByFlightIdAsync(flightId);
        if (currentCrewIds.Contains(employeeId))
        {
            throw new InvalidOperationException("Employee already assigned to this flight.");
        }

        if (!await IsEmployeeAvailableAsync(employeeId, flight.Date, flight.Route.Id, flight.Id))
        {
            throw new InvalidOperationException($"Conflict: Employee {employee.Name} is already assigned to another flight during this time.");
        }

        await employeeFlightRepository.AssignAsync(employeeId, flightId);
    }

    public async Task RemoveEmployeeFromFlightUsingIdsAsync(int flightId, int employeeId)
    {
        await employeeFlightRepository.UnassignAsync(employeeId, flightId);
    }

    public async Task RemoveAllCrewAssignmentsForFlightAsync(int flightId)
    {
        if (flightId > 0)
        {
            await employeeFlightRepository.DeleteByFlightIdAsync(flightId);
        }
    }

    public async Task RemoveAllFlightsAssignmentsForEmployeeAsync(int employeeId)
    {
        if (employeeId > 0)
        {
            await employeeFlightRepository.DeleteByEmployeeIdAsync(employeeId);
        }
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAssignedToFlightAsync(int flightId)
    {
        List<Employee> flightCrew = new List<Employee>();
        IEnumerable<int> crewIdentifiers = await employeeFlightRepository.GetEmployeeIdsByFlightIdAsync(flightId);

        foreach (int identifier in crewIdentifiers)
        {
            Employee? employee = await employeeRepository.GetByIdAsync(identifier);
            if (employee != null)
            {
                flightCrew.Add(employee);
            }
        }

        return flightCrew;
    }

    public string FormatCrewList(int flightId)
    {
        IEnumerable<Employee> crew = GetEmployeesAssignedToFlightAsync(flightId).GetAwaiter().GetResult();
        List<Employee> crewList = crew.ToList();

        if (crewList.Count == 0)
        {
            return UnnasignedCrew;
        }

        StringBuilder crewNames = new StringBuilder();
        for (int index = 0; index < crewList.Count; index++)
        {
            crewNames.Append(crewList[index].Name);
            if (index < crewList.Count - 1)
            {
                crewNames.Append(", ");
            }
        }

        return crewNames.ToString();
    }

    public async Task<IEnumerable<Flight>> GetEmployeeScheduleAsync(int employeeId)
    {
        if (employeeId <= 0)
        {
            return new List<Flight>();
        }

        IEnumerable<int> assignedFlightIdentifiers = await employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(employeeId);
        List<Flight> scheduledFlights = new List<Flight>();

        foreach (int identifier in assignedFlightIdentifiers)
        {
            Flight? flight = await flightRepository.GetByIdAsync(identifier);
            if (flight != null)
            {
                scheduledFlights.Add(flight);
            }
        }

        return scheduledFlights;
    }

    public async Task<IEnumerable<EmployeeScheduleItem>> GetFormattedEmployeeScheduleAsync(int employeeId)
    {
        List<EmployeeScheduleItem> scheduleItems = new List<EmployeeScheduleItem>();
        if (employeeId <= 0)
        {
            return scheduleItems;
        }

        List<Flight> flights = (await GetEmployeeScheduleAsync(employeeId)).ToList();
        flights.Sort(new FlightDateComparer());

        foreach (Flight flight in flights)
        {
            Route? route = await routeRepository.GetByIdAsync(flight.Route.Id);

            Gate? gate = null;
            if (flight.Gate != null && flight.Gate.Id > 0)
            {
                gate = await gateService.GetGateByIdAsync(flight.Gate.Id);
            }

            Runway? runway = null;
            if (flight.Runway != null)
            {
                runway = await runwayService.GetRunwayByIdAsync(flight.Runway.Id);
            }

            scheduleItems.Add(new EmployeeScheduleItem
            {
                Id = flight.Id.ToString(),
                FlightNumber = flight.FlightNumber,
                FlightType = routeService.NormalizeFlightType(route?.RouteType),
                Date = flight.Date.ToString(FlightDateFormat),
                GateName = gate?.GateName ?? EmptyFieldPlaceholder,
                RunwayName = runway?.Name ?? EmptyFieldPlaceholder,
                FlightTime = routeService.GetRelevantTime(route)
            });
        }

        return scheduleItems;
    }

    public async Task<bool> IsEmployeeAvailableAsync(int employeeId, DateTime targetDate, int targetRouteId, int? excludedFlightId = null)
    {
        Route? targetRoute = await routeRepository.GetByIdAsync(targetRouteId);
        if (targetRoute == null)
        {
            return false;
        }

        IEnumerable<Flight> completeSchedule = await GetEmployeeScheduleAsync(employeeId);

        foreach (Flight scheduledFlight in completeSchedule)
        {
            if (scheduledFlight.Date.Date == targetDate.Date && scheduledFlight.Id != excludedFlightId)
            {
                Route? scheduledRoute = await routeRepository.GetByIdAsync(scheduledFlight.Route.Id);

                if (scheduledRoute != null)
                {
                    bool isTimeOverlap = targetRoute.DepartureTime < scheduledRoute.ArrivalTime &&
                                         targetRoute.ArrivalTime > scheduledRoute.DepartureTime;

                    if (isTimeOverlap)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public async Task AssignEmpolyeesToFlightUsingIdsAsync(int flightId, List<int> employeeIds)
    {
        foreach (int employeeId in employeeIds)
        {
            try
            {
                await AssignEmployeeToFlightUsingIdsAsync(flightId, employeeId);
            }
            catch
            {
            }
        }
    }

    public async Task UpdateEmployeesForFlightUsingIdsAsync(int flightId, List<int> updatedEmployeeIds)
    {
        IEnumerable<int> existingCrewIds = await employeeFlightRepository.GetEmployeeIdsByFlightIdAsync(flightId);
        List<int> existingList = existingCrewIds.ToList();

        foreach (int existingId in existingList)
        {
            if (!updatedEmployeeIds.Contains(existingId))
            {
                await RemoveEmployeeFromFlightUsingIdsAsync(flightId, existingId);
            }
        }

        foreach (int newId in updatedEmployeeIds)
        {
            if (!existingList.Contains(newId))
            {
                await AssignEmployeeToFlightUsingIdsAsync(flightId, newId);
            }
        }
    }

    public async Task<IEnumerable<CrewMemberSelectionData>> GetCrewSelectionDataAsync(Flight flight)
    {
        IEnumerable<int> assignedEmployeeIds = await employeeFlightRepository.GetEmployeeIdsByFlightIdAsync(flight.Id);
        List<Employee> availableEmployees = (await GetAvailableEmployeesGroupedByRoleAsync(flight)).ToList();

        List<CrewMemberSelectionData> result = new List<CrewMemberSelectionData>();
        EmployeeRoleEnum? previousRole = null;

        foreach (Employee candidate in availableEmployees)
        {
            EmployeeRoleEnum currentRole = candidate.Role;
            bool isFirstInGroup = currentRole != previousRole;

            result.Add(new CrewMemberSelectionData
            {
                Employee = candidate,
                IsSelected = assignedEmployeeIds.Contains(candidate.Id),
                IsFirstInRoleGroup = isFirstInGroup,
                RoleHeader = currentRole.ToString()
            });

            previousRole = currentRole;
        }

        return result;
    }

    public async Task<IEnumerable<CrewMemberSelectionData>> GetCrewSelectionDataByIdAsync(int flightId)
    {
        Flight? flight = await flightRepository.GetByIdAsync(flightId);

        if (flight == null)
        {
            return new List<CrewMemberSelectionData>();
        }

        return await GetCrewSelectionDataAsync(flight);
    }

    public async Task<IEnumerable<Employee>> GetAvailableEmployeesGroupedByRoleAsync(Flight flight)
    {
        IEnumerable<Employee> allEmployees = await employeeRepository.GetAsync();
        List<Employee> availableEmployees = new List<Employee>();

        foreach (Employee candidate in allEmployees)
        {
            if (await IsEmployeeAvailableAsync(candidate.Id, flight.Date, flight.Route.Id, flight.Id))
            {
                availableEmployees.Add(candidate);
            }
        }

        availableEmployees.Sort(new EmployeeRoleAndNameComparer());
        return availableEmployees;
    }

    public async Task<IEnumerable<Employee>> GetAvailableEmployeesGroupedByRoleByIdAsync(int flightId)
    {
        Flight? flight = await flightRepository.GetByIdAsync(flightId);

        if (flight == null)
        {
            return new List<Employee>();
        }

        return await GetAvailableEmployeesGroupedByRoleAsync(flight);
    }

    private class FlightDateComparer : IComparer<Flight>
    {
        public int Compare(Flight? firstFlight, Flight? secondFlight)
        {
            if (firstFlight == null || secondFlight == null)
            {
                return 0;
            }

            return firstFlight.Date.CompareTo(secondFlight.Date);
        }
    }

    private class EmployeeRoleAndNameComparer : IComparer<Employee>
    {
        public int Compare(Employee? firstEmployee, Employee? secondEmployee)
        {
            if (firstEmployee == null || secondEmployee == null)
            {
                return 0;
            }
            int roleComparison = firstEmployee.Role.CompareTo(secondEmployee.Role);

            if (roleComparison != 0)
            {
                return roleComparison;
            }

            return string.Compare(firstEmployee.Name, secondEmployee.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
