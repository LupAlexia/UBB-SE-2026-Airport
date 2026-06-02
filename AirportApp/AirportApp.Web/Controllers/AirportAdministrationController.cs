using Microsoft.AspNetCore.Mvc;
using AirportLib.Domain.User;
using AirportApp.Web.Infrastructure;
using AirportApp.Web.Models.AirportManagement;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Controllers;

[RequireAirportRole(AirportModuleRole.AirportAdministrator)]
public class AirportAdministrationController(
    WebUserSession userSession,
    IFlightRouteService flightRouteService,
    IFlightService flightService,
    IEmployeeService employeeService,
    IEmployeeFlightService employeeFlightService,
    IAirportService airportService,
    IRunwayService runwayService,
    IGateService gateService,
    ICompanyService companyService) : Controller
{
    public IActionResult Index()
    {
        return this.RedirectToAction(nameof(this.DisplayFlights));
    }

    [HttpGet]
    public IActionResult DisplayFlights(string? search)
    {
        List<Flight> allFlightsList = flightRouteService.GetAllFlightsWithDetails();

        if (!string.IsNullOrWhiteSpace(search))
        {
            allFlightsList = flightRouteService.SearchFlights(allFlightsList, search);
        }

        List<FlightSummary> flightSummaries = new List<FlightSummary>();
        foreach (Flight flightInstance in allFlightsList)
        {
            string formattedCrewText = employeeFlightService.FormatCrewList(flightInstance.Id);
            flightSummaries.Add(flightRouteService.BuildFlightSummary(flightInstance, formattedCrewText));
        }

        FlightsDashboardViewModel viewModel = new FlightsDashboardViewModel
        {
            Flights = flightSummaries,
            SearchQuery = search ?? string.Empty
        };

        return this.View(viewModel);
    }

    [HttpGet]
    public IActionResult OpenAddFlightForm()
    {
        const int NewFlightCompanyContext = 0;
        return this.View(this.CreateAddFlightFormViewModel(NewFlightCompanyContext));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExecuteAddFlight(AddFlightFormModel form)
    {
        form.FlightNumberPrefix = companyService.GenerateFlightCodeUsingCompanyId(form.CompanyId);
        this.ModelState.Remove(nameof(AddFlightFormModel.FlightNumberPrefix));

        if (!this.ModelState.IsValid)
        {
            form.Airports = airportService.GetAllAirports();
            form.Runways = runwayService.GetAllRunways();
            form.Gates = gateService.GetAllGates();
            return this.View(nameof(this.OpenAddFlightForm), form);
        }

        flightRouteService.CreateFlightWithSchedule(
            form.CompanyId,
            form.RouteType,
            form.AirportId,
            form.Capacity,
            TimeSpan.FromMinutes(form.DepartureOffsetMinutes),
            TimeSpan.FromMinutes(form.ArrivalOffsetMinutes),
            form.IsRecurrent,
            form.StartDate,
            form.EndDate,
            form.SingleDate,
            form.RecurrenceType,
            form.CustomDaysText,
            form.RunwayId,
            form.GateId,
            _ => form.FlightNumberPrefix);

        return this.RedirectToAction(nameof(this.DisplayFlights));
    }

    [HttpGet]
    public IActionResult DeleteFlight(int flightId)
    {
        if (flightId <= 0)
        {
            return this.NotFound();
        }

        Flight? flightInstance = flightRouteService.GetFlightById(flightId);

        if (flightInstance == null)
        {
            return this.NotFound("Flight not found or access denied.");
        }

        string crewListText = employeeFlightService.FormatCrewList(flightId);
        FlightSummary viewModel = flightRouteService.BuildFlightSummary(flightInstance, crewListText);

        return this.View(viewModel);
    }

    [HttpPost]
    [ActionName("DeleteFlight")]
    [ValidateAntiForgeryToken]
    public IActionResult ExecuteDeleteFlight(int flightId)
    {
        employeeFlightService.RemoveAllCrewAssignmentsForFlight(flightId);
        flightRouteService.DeleteFlightUsingId(flightId);

        return this.RedirectToAction(nameof(this.Index));
    }

    [HttpGet]
    public IActionResult OpenCrewManagement(int flightId)
    {
        Flight? flightInstance = flightRouteService.GetFlightById(flightId);

        if (flightInstance == null)
        {
            return this.NotFound();
        }

        string crewListText = employeeFlightService.FormatCrewList(flightId);
        List<Employee> assignedEmployees = employeeFlightService.GetEmployeesAssignedToFlight(flightId);
        List<int> assignedEmployeeIds = new List<int>();

        foreach (Employee employee in assignedEmployees)
        {
            assignedEmployeeIds.Add(employee.Id);
        }

        CrewManagementViewModel viewModel = new CrewManagementViewModel
        {
            FlightId = flightId,
            Flight = flightRouteService.BuildFlightSummary(flightInstance, crewListText),
            CrewCandidates = employeeFlightService.GetCrewSelectionDataById(flightId),
            SelectedEmployeeIds = assignedEmployeeIds
        };

        return this.View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveCrewAssignment(int flightId, List<int> selectedEmployeeIds)
    {
        List<int> validEmployeeIds = selectedEmployeeIds ?? new List<int>();
        employeeFlightService.UpdateEmployeesForFlightUsingIds(flightId, validEmployeeIds);

        return this.RedirectToAction(nameof(this.DisplayFlights));
    }

    [HttpGet]
    public IActionResult DisplayEmployees()
    {
        List<EmployeeRoleEnum> rolesToDisplay = new List<EmployeeRoleEnum>();
        foreach (EmployeeRoleEnum role in Enum.GetValues(typeof(EmployeeRoleEnum)))
        {
            if (role != EmployeeRoleEnum.Other)
            {
                rolesToDisplay.Add(role);
            }
        }

        EmployeesDashboardViewModel viewModel = new EmployeesDashboardViewModel
        {
            Employees = employeeService.GetAllEmployees(),
            DisplayedRoles = rolesToDisplay
        };

        return this.View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExecuteAddEmployee(EmployeeFormModel form)
    {
        if (!this.ModelState.IsValid)
        {
            return this.RedirectToAction(nameof(this.DisplayEmployees));
        }

        EmployeeRoleEnum assignedRole = employeeService.ParseRole(form.Role);

        DateOnly birthDate = form.Birthday.HasValue
            ? DateOnly.FromDateTime(form.Birthday.Value)
            : DateOnly.FromDateTime(DateTime.Today.AddYears(-20));

        DateOnly hireDate = form.HiringDate.HasValue
            ? DateOnly.FromDateTime(form.HiringDate.Value)
            : DateOnly.FromDateTime(DateTime.Today);

        employeeService.AddEmployee(
            form.Name,
            assignedRole,
            birthDate,
            form.Salary,
            hireDate);

        return this.RedirectToAction(nameof(this.DisplayEmployees));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExecuteRemoveEmployee(int employeeId)
    {
        employeeService.DeleteWithAssignments(employeeId);
        return this.RedirectToAction(nameof(this.DisplayEmployees));
    }

    [HttpGet]
    public IActionResult DisplayConfiguration()
    {
        AirportAdminViewModel viewModel = new AirportAdminViewModel
        {
            RunwaysList = runwayService.GetAllRunways(),
            GatesList = gateService.GetAllGates(),
            AirportsList = airportService.GetAllAirports()
        };

        return this.View(viewModel);
    }

    [HttpGet]
    public IActionResult OpenEditRunwayForm(int id)
    {
        Runway? runway = runwayService.GetRunwayById(id);
        if (runway == null)
        {
            return this.NotFound();
        }

        return this.View(runway);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExecuteUpdateRunway(Runway runway)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(nameof(this.OpenEditRunwayForm), runway);
        }

        runwayService.UpdateRunway(runway.Id, runway.Name, runway.HandleTime);
        return this.RedirectToAction(nameof(this.DisplayConfiguration));
    }

    [HttpGet]
    public IActionResult OpenEditGateForm(int id)
    {
        Gate? gate = gateService.GetGateById(id);
        if (gate == null)
        {
            return this.NotFound();
        }

        return this.View(gate);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExecuteUpdateGate(Gate gate)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(nameof(this.OpenEditGateForm), gate);
        }

        gateService.UpdateGate(gate.Id, gate.Name);
        return this.RedirectToAction(nameof(this.DisplayConfiguration));
    }

    private AddFlightFormModel CreateAddFlightFormViewModel(int companyId)
    {
        return new AddFlightFormModel
        {
            CompanyId = companyId,
            Airports = airportService.GetAllAirports(),
            Runways = runwayService.GetAllRunways(),
            Gates = gateService.GetAllGates()
        };
    }

    [HttpGet]
    public IActionResult OpenAddEmployeeForm(EmployeeRoleEnum role)
    {
        if (role == EmployeeRoleEnum.Other)
        {
            return this.BadRequest("A specific role is required to add an employee.");
        }

        const int DefaultStartingSalary = 0;
        DateTime adultBirthdayDefault = DateTime.Today.AddYears(-20);

        EmployeeFormModel viewModel = new EmployeeFormModel
        {
            Id = 0,
            Role = role.ToString(),
            Name = "New Employee",
            Salary = DefaultStartingSalary,
            Birthday = adultBirthdayDefault,
            HiringDate = DateTime.Today
        };

        return this.View(viewModel);
    }

    [HttpGet]
    public IActionResult OpenEditEmployeeForm(int employeeId)
    {
        if (employeeId <= 0)
        {
            return this.NotFound();
        }

        Employee? employee = employeeService.GetEmployeeById(employeeId);

        if (employee == null)
        {
            return this.NotFound();
        }

        EmployeeFormModel viewModel = new EmployeeFormModel
        {
            Id = employee.Id,
            Name = employee.Name,
            Role = employee.Role.ToString(),

            Salary = employee.Salary,

            Birthday = employee.Birthday.ToDateTime(TimeOnly.MinValue),
            HiringDate = employee.HiringDate.ToDateTime(TimeOnly.MinValue)
        };

        return this.View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExecuteSaveEmployee(EmployeeFormModel form)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(form.Id == 0 ? nameof(this.OpenAddEmployeeForm) : nameof(this.OpenEditEmployeeForm), form);
        }

        EmployeeRoleEnum assignedRole = employeeService.ParseRole(form.Role);

        if (form.Id == 0)
        {
            employeeService.AddEmployee(
                form.Name,
                assignedRole,
                DateOnly.FromDateTime(form.Birthday!.Value),
                form.Salary,
                DateOnly.FromDateTime(form.HiringDate!.Value));
        }
        else
        {
            employeeService.UpdateEmployee(
                form.Id,
                name: form.Name,
                role: assignedRole,
                salary: form.Salary,
                birthday: DateOnly.FromDateTime(form.Birthday!.Value),
                hiringDate: DateOnly.FromDateTime(form.HiringDate!.Value));
        }

        return this.RedirectToAction(nameof(this.DisplayEmployees));
    }

    [HttpGet]
    public IActionResult CreateRunway()
    {
        return this.View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateRunway(Runway runway)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(runway);
        }

        runwayService.AddRunway(runway.Name, runway.HandleTime);
        return this.RedirectToAction(nameof(this.DisplayConfiguration));
    }

    [HttpGet]
    public IActionResult EditRunway(int id)
    {
        Runway? runway = runwayService.GetRunwayById(id);
        if (runway == null)
        {
            return this.NotFound();
        }
        return this.View(runway);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditRunway(Runway runway)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(runway);
        }

        runwayService.UpdateRunway(runway.Id, runway.Name, runway.HandleTime);
        return this.RedirectToAction(nameof(this.DisplayConfiguration));
    }

    [HttpGet]
    public IActionResult CreateGate()
    {
        return this.View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateGate(Gate gate)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(gate);
        }

        gateService.AddGate(gate.Name);
        return this.RedirectToAction(nameof(this.DisplayConfiguration));
    }

    [HttpGet]
    public IActionResult EditGate(int id)
    {
        Gate? gate = gateService.GetGateById(id);
        if (gate == null)
        {
            return this.NotFound();
        }
        return this.View(gate);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditGate(Gate gate)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(gate);
        }

        gateService.UpdateGate(gate.Id, gate.Name);
        return this.RedirectToAction(nameof(this.DisplayConfiguration));
    }

    [HttpGet]
    public IActionResult DeleteRunway(int id)
    {
        Runway? runway = runwayService.GetRunwayById(id);
        if (runway == null)
        {
            return this.NotFound();
        }

        this.ViewData["WarningMessage"] = runwayService.GetDeleteWarningMessage(id);
        return this.View(runway);
    }

    [HttpPost]
    [ActionName("DeleteRunway")]
    [ValidateAntiForgeryToken]
    public IActionResult ExecuteDeleteRunway(int id)
    {
        runwayService.DeleteRunwayUsingId(id);
        return this.RedirectToAction(nameof(this.DisplayConfiguration));
    }

    [HttpGet]
    public IActionResult DeleteGate(int id)
    {
        Gate? gate = gateService.GetGateById(id);
        if (gate == null)
        {
            return this.NotFound();
        }

        this.ViewData["WarningMessage"] = gateService.GetDeleteWarningMessage(id);
        return this.View(gate);
    }

    [HttpPost]
    [ActionName("DeleteGate")]
    [ValidateAntiForgeryToken]
    public IActionResult ExecuteDeleteGate(int id)
    {
        gateService.DeleteGateUsingId(id);
        return this.RedirectToAction(nameof(this.DisplayConfiguration));
    }

    [HttpGet]
    public IActionResult DeleteAirport(int id)
    {
        Airport? airport = airportService.GetAirportById(id);
        if (airport == null)
        {
            return this.NotFound();
        }

        bool hasFlights = airportService.HasFlights(id);
        this.ViewData["WarningMessage"] = hasFlights
            ? $"CRITICAL: '{airport.Name}' has flights assigned. Deleting it will remove ALL associated flights. Proceed?"
            : $"Are you sure you want to remove '{airport.Name}' from destinations?";

        return this.View(airport);
    }

    [HttpPost]
    [ActionName("DeleteAirport")]
    [ValidateAntiForgeryToken]
    public IActionResult ExecuteDeleteAirport(int id)
    {
        airportService.DeleteAirportUsingId(id);
        return this.RedirectToAction(nameof(this.DisplayConfiguration));
    }

    [HttpGet]
    public IActionResult CreateAirport()
    {
        return this.View(new Airport());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateAirport(Airport airport)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(airport);
        }

        airportService.AddAirport(airport.Code, airport.Name, airport.City);

        return this.RedirectToAction(nameof(this.DisplayConfiguration));
    }

    [HttpGet]
    public IActionResult EditAirport(int id)
    {
        Airport? airport = airportService.GetAirportById(id);

        if (airport == null)
        {
            return this.NotFound();
        }

        return this.View(airport);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditAirport(Airport airport)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(airport);
        }

        airportService.UpdateAirport(
            airport.Id,
            newName: airport.Name,
            newCity: airport.City,
            newCode: airport.Code);

        return this.RedirectToAction(nameof(this.DisplayConfiguration));
    }
}
