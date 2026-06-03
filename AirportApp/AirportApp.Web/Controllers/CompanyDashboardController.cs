using System.Security.Claims;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Web.Models.AirportManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Web.Controllers;

[Authorize(Roles = "Manager")]
public class CompanyDashboardController : Controller
{
    private readonly IFlightRouteService flightRouteService;
    private readonly ICompanyService companyService;
    private readonly IEmployeeFlightService employeeFlightService;
    private readonly IEmployeeService employeeService;
    private readonly IAirportService airportService;
    private readonly IRunwayService runwayService;
    private readonly IGateService gateService;

    public CompanyDashboardController(
        IFlightRouteService flightRouteService,
        ICompanyService companyService,
        IEmployeeFlightService employeeFlightService,
        IEmployeeService employeeService,
        IAirportService airportService,
        IRunwayService runwayService,
        IGateService gateService)
    {
        this.flightRouteService = flightRouteService;
        this.companyService = companyService;
        this.employeeFlightService = employeeFlightService;
        this.employeeService = employeeService;
        this.airportService = airportService;
        this.runwayService = runwayService;
        this.gateService = gateService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? companyId, string? search)
    {
        List<Company> companies = await GetManagerCompaniesAsync();

        if (companies.Count == 0)
            return Forbid();

        int selectedCompanyId = companyId ?? companies[0].Id;

        if (!companies.Any(company => company.Id == selectedCompanyId))
            return Forbid();

        return View(await BuildDashboardModelAsync(selectedCompanyId, companies, search));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFlight([Bind(Prefix = "AddFlightForm")] AddFlightFormModel form, int? companyId)
    {
        List<Company> companies = await GetManagerCompaniesAsync();
        int selectedCompanyId = companyId ?? companies.FirstOrDefault()?.Id ?? 0;
        form.CompanyId = selectedCompanyId;

        if (selectedCompanyId == 0 || !companies.Any(company => company.Id == selectedCompanyId))
            return Forbid();

        try
        {
            TimeSpan departureOffset = CalculateOffset(form.DepartureHour, form.DepartureMinute, form.DepartureAmPm);
            TimeSpan arrivalOffset = CalculateOffset(form.ArrivalHour, form.ArrivalMinute, form.ArrivalAmPm);

            ModelState.Clear();
            TryValidateModel(form);

            if (!ModelState.IsValid)
            {
                await PopulateAddFlightDropdownsAsync(form);
                return View(nameof(Index), await BuildDashboardModelAsync(selectedCompanyId, companies, null, form, true));
            }

            string flightCode = await companyService.GenerateFlightCodeUsingCompanyIdAsync(selectedCompanyId);

            await flightRouteService.CreateFlightWithScheduleAsync(
                selectedCompanyId,
                form.RouteType,
                form.AirportId,
                form.Capacity,
                departureOffset,
                arrivalOffset,
                form.IsRecurrent,
                form.StartDate,
                form.EndDate,
                form.SingleDate,
                form.RecurrenceType,
                form.CustomDaysText,
                form.RunwayId,
                form.GateId,
                _ => flightCode);

            return RedirectToAction(nameof(Index), new { companyId = selectedCompanyId });
        }
        catch (Exception exception)
        {
            await PopulateAddFlightDropdownsAsync(form);
            ModelState.AddModelError(string.Empty, exception.GetBaseException().Message);
            return View(nameof(Index), await BuildDashboardModelAsync(selectedCompanyId, companies, null, form, true));
        }
    }

    [HttpGet]
    public async Task<IActionResult> DeleteFlight(int flightId)
    {
        if (flightId <= 0)
            return NotFound();

        Task<Flight?> flightTask = flightRouteService.GetFlightByIdAsync(flightId);
        Task<List<Company>> companiesTask = GetManagerCompaniesAsync();
        await Task.WhenAll(flightTask, companiesTask);

        Flight? flight = flightTask.Result;
        List<Company> companies = companiesTask.Result;

        if (flight == null || !CanAccessFlight(flight, companies))
            return NotFound("Flight not found or access denied.");

        string crewListText = employeeFlightService.FormatCrewList(flightId);
        FlightSummary viewModel = await flightRouteService.BuildFlightSummaryAsync(flight, crewListText);

        return View(viewModel);
    }

    [HttpPost]
    [ActionName("DeleteFlight")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExecuteDeleteFlight(int flightId)
    {
        Task<Flight?> flightTask = flightRouteService.GetFlightByIdAsync(flightId);
        Task<List<Company>> companiesTask = GetManagerCompaniesAsync();
        await Task.WhenAll(flightTask, companiesTask);

        Flight? flight = flightTask.Result;

        if (flight == null || !CanAccessFlight(flight, companiesTask.Result))
            return Forbid();

        await employeeFlightService.RemoveAllCrewAssignmentsForFlightAsync(flightId);
        await flightRouteService.DeleteFlightUsingIdAsync(flightId);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ManageCrew(int flightId)
    {
        Task<Flight?> flightTask = flightRouteService.GetFlightByIdAsync(flightId);
        Task<List<Company>> companiesTask = GetManagerCompaniesAsync();
        await Task.WhenAll(flightTask, companiesTask);

        Flight? flight = flightTask.Result;

        if (flight == null)
            return NotFound();

        if (!CanAccessFlight(flight, companiesTask.Result))
            return StatusCode(StatusCodes.Status403Forbidden);

        IEnumerable<CrewMemberSelectionData> crewData = await employeeFlightService.GetCrewSelectionDataByIdAsync(flightId);
        string crewText = employeeFlightService.FormatCrewList(flightId);
        FlightSummary summary = await flightRouteService.BuildFlightSummaryAsync(flight, crewText);
        IEnumerable<Employee> assignedEmployees = await employeeFlightService.GetEmployeesAssignedToFlightAsync(flightId);

        var model = new CrewManagementViewModel
        {
            FlightId = flightId,
            Flight = summary,
            CrewCandidates = crewData.ToList(),
            SelectedEmployeeIds = assignedEmployees.Select(employee => employee.Id).ToList(),
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetCrewModal(int flightId)
    {
        Flight? flight = await flightRouteService.GetFlightByIdAsync(flightId);
        if (flight == null) return NotFound();

        List<Company> companies = await GetManagerCompaniesAsync();
        if (!CanAccessFlight(flight, companies)) return Forbid();

        Task<IEnumerable<Employee>> allEmployeesTask = employeeService.GetAllEmployeesAsync();
        Task<IEnumerable<Employee>> assignedTask = employeeFlightService.GetEmployeesAssignedToFlightAsync(flightId);
        Task<IEnumerable<CrewMemberSelectionData>> availableTask = employeeFlightService.GetCrewSelectionDataByIdAsync(flightId);
        await Task.WhenAll(allEmployeesTask, assignedTask, availableTask);

        List<int> assignedIds = assignedTask.Result.Select(e => e.Id).ToList();
        HashSet<int> availableIds = availableTask.Result.Select(c => c.Employee.Id).ToHashSet();

        EmployeeRoleEnum? prevRole = null;
        var crewCandidates = new List<CrewMemberSelectionData>();

        foreach (Employee emp in allEmployeesTask.Result.OrderBy(e => e.Role).ThenBy(e => e.Name))
        {
            crewCandidates.Add(new CrewMemberSelectionData
            {
                Employee = emp,
                IsSelected = assignedIds.Contains(emp.Id),
                IsFirstInRoleGroup = emp.Role != prevRole,
                RoleHeader = emp.Role.ToString()
            });
            prevRole = emp.Role;
        }

        var model = new CrewManagementViewModel
        {
            FlightId = flightId,
            CrewCandidates = crewCandidates,
            SelectedEmployeeIds = assignedIds,
            AvailableEmployeeIds = availableIds,
        };

        return PartialView("_CrewModal", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCrew(int flightId, int companyId, List<int> selectedEmployeeIds)
    {
        Task<Flight?> flightTask = flightRouteService.GetFlightByIdAsync(flightId);
        Task<List<Company>> companiesTask = GetManagerCompaniesAsync();
        await Task.WhenAll(flightTask, companiesTask);

        Flight? flight = flightTask.Result;

        if (flight == null || !CanAccessFlight(flight, companiesTask.Result))
            return StatusCode(StatusCodes.Status403Forbidden);

        await employeeFlightService.UpdateEmployeesForFlightUsingIdsAsync(flightId, selectedEmployeeIds ?? new List<int>());
        return RedirectToAction(nameof(Index), new { companyId });
    }

    private int GetManagerId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private async Task<List<Company>> GetManagerCompaniesAsync() =>
        (await companyService.GetCompaniesByManagerIdAsync(GetManagerId())).ToList();

    private static bool CanAccessFlight(Flight flight, IEnumerable<Company> companies) =>
        companies.Any(company => company.Id == flight.Route?.Company?.Id);

    private static TimeSpan CalculateOffset(int hour, int minute, string amPm)
    {
        int militaryHour = hour % 12;
        if (string.Equals(amPm, "PM", StringComparison.OrdinalIgnoreCase))
            militaryHour += 12;
        return new TimeSpan(militaryHour, minute, 0);
    }

    private async Task<CompanyDashboardViewModel> BuildDashboardModelAsync(
        int selectedCompanyId,
        List<Company> companies,
        string? search,
        AddFlightFormModel? addFlightForm = null,
        bool showAddFlightForm = false)
    {
        Company? selected = companies.FirstOrDefault(company => company.Id == selectedCompanyId);
        IEnumerable<Flight> allFlights = await flightRouteService.GetFlightsByCompanyIdAsync(selectedCompanyId);

        if (!string.IsNullOrWhiteSpace(search))
            allFlights = await flightRouteService.SearchFlightsAsync(allFlights, search);

        var summaries = new List<FlightSummary>();
        foreach (Flight flight in allFlights)
        {
            string crewText = employeeFlightService.FormatCrewList(flight.Id);
            summaries.Add(await flightRouteService.BuildFlightSummaryAsync(flight, crewText));
        }

        return new CompanyDashboardViewModel
        {
            CompanyId = selectedCompanyId,
            CompanyName = selected?.Name ?? string.Empty,
            ManagerCompanies = companies,
            Flights = summaries,
            SearchQuery = search ?? string.Empty,
            AddFlightForm = addFlightForm ?? await BuildAddFlightFormAsync(selectedCompanyId),
            ShowAddFlightForm = showAddFlightForm,
        };
    }

    private async Task PopulateAddFlightDropdownsAsync(AddFlightFormModel form)
    {
        Task<IEnumerable<Airport>> airportsTask = airportService.GetAllAirportsAsync();
        Task<IEnumerable<Runway>> runwaysTask = runwayService.GetAllRunwaysAsync();
        Task<IEnumerable<Gate>> gatesTask = gateService.GetAllGatesAsync();

        await Task.WhenAll(airportsTask, runwaysTask, gatesTask);

        form.Airports = airportsTask.Result.ToList();
        form.Runways = runwaysTask.Result.ToList();
        form.Gates = gatesTask.Result.ToList();
    }

    private async Task<AddFlightFormModel> BuildAddFlightFormAsync(int companyId)
    {
        var form = new AddFlightFormModel { CompanyId = companyId };
        await PopulateAddFlightDropdownsAsync(form);
        return form;
    }
}

