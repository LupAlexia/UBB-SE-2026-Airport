using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using AirportApp.ClassLibrary.Service.Interface;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class EmployeeFlightServiceTests
{
    private const int TargetFlightId = 1;
    private const int TargetEmployeeId = 1;
    private const int TargetRouteId = 1;
    private const int ConflictingFlightId = 99;
    private const int ConflictingRouteId = 2;
    private const int NextDayFlightId = 50;
    private const int InvalidId = 0;
    private const int MissingEmployeeId = 2;
    private const int MissingFlightId = 2;
    private const int MissingRouteId = 99;
    private const int FailingEmployeeId = 1;
    private const int SucceedingEmployeeId = 2;
    private const int EmployeeToRemoveId = 1;
    private const int EmployeeToKeepId = 2;
    private const int EmployeeToAddId = 3;
    private const int AvailableEmployeeId = 1;
    private const int UnavailableEmployeeId = 2;
    private const int TargetDepartureHour = 10;
    private const int TargetArrivalHour = 12;
    private const int ConflictingDepartureHour = 11;
    private const int ConflictingArrivalHour = 13;
    private const int PilotAliceId = 2;
    private const int PilotCharlieId = 3;
    private const int CoPilotBobId = 1;
    private const string DefaultFlightCode = "FL-1000";
    private const string PlaceholderDash = "-";
    private const string PilotAliceName = "Alice";
    private const string PilotCharlieName = "Charlie";
    private const string CoPilotBobName = "Bob";
    private const string FutureFlightType = "X";
    private const int FutureFlightId = 1;
    private const int CurrentFlightId = 2;

    private static EmployeeFlightService CreateTestService(
        IEmployeeFlightRepository? employeeFlightRepository = null,
        IEmployeeRepository? employeeRepository = null,
        IFlightRepository? flightRepository = null,
        IRouteRepository? routeRepository = null,
        IGateService? gateService = null,
        IRunwayService? runwayService = null,
        IRouteService? routeService = null)
    {
        return new EmployeeFlightService(
            employeeFlightRepository ?? Substitute.For<IEmployeeFlightRepository>(),
            employeeRepository ?? Substitute.For<IEmployeeRepository>(),
            flightRepository ?? Substitute.For<IFlightRepository>(),
            routeRepository ?? Substitute.For<IRouteRepository>(),
            gateService ?? Substitute.For<IGateService>(),
            runwayService ?? Substitute.For<IRunwayService>(),
            routeService ?? Substitute.For<IRouteService>());
    }

    [Test]
    public void AssignEmployeeToFlightUsingIdsAsync_FlightIdIsInvalid_ThrowsArgumentException()
    {
        var service = CreateTestService();

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.AssignEmployeeToFlightUsingIdsAsync(InvalidId, TargetEmployeeId));
    }

    [Test]
    public void AssignEmployeeToFlightUsingIdsAsync_EmployeeIdIsInvalid_ThrowsArgumentException()
    {
        var service = CreateTestService();

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.AssignEmployeeToFlightUsingIdsAsync(TargetFlightId, InvalidId));
    }

    [Test]
    public void AssignEmployeeToFlightUsingIdsAsync_EmployeeDoesNotExist_ThrowsInvalidOperationException()
    {
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        employeeRepository.GetByIdAsync(TargetEmployeeId).Returns(Task.FromResult<Employee?>(null));
        flightRepository.GetByIdAsync(TargetFlightId).Returns(Task.FromResult<Flight?>(new Flight()));

        var service = CreateTestService(employeeRepository: employeeRepository, flightRepository: flightRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignEmployeeToFlightUsingIdsAsync(TargetFlightId, TargetEmployeeId));
    }

    [Test]
    public void AssignEmployeeToFlightUsingIdsAsync_FlightDoesNotExist_ThrowsInvalidOperationException()
    {
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        employeeRepository.GetByIdAsync(TargetEmployeeId).Returns(Task.FromResult<Employee?>(new Employee()));
        flightRepository.GetByIdAsync(TargetFlightId).Returns(Task.FromResult<Flight?>(null));

        var service = CreateTestService(employeeRepository: employeeRepository, flightRepository: flightRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignEmployeeToFlightUsingIdsAsync(TargetFlightId, TargetEmployeeId));
    }

    [Test]
    public void AssignEmployeeToFlightUsingIdsAsync_EmployeeIsAlreadyAssigned_ThrowsInvalidOperationException()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();

        employeeRepository.GetByIdAsync(TargetEmployeeId).Returns(Task.FromResult<Employee?>(new Employee()));
        flightRepository.GetByIdAsync(TargetFlightId).Returns(Task.FromResult<Flight?>(
            new Flight { Id = TargetFlightId, Route = new Route { Id = TargetRouteId }, Date = DateTime.Today }));
        employeeFlightRepository.GetEmployeeIdsByFlightIdAsync(TargetFlightId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int> { TargetEmployeeId }));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            employeeRepository: employeeRepository,
            flightRepository: flightRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignEmployeeToFlightUsingIdsAsync(TargetFlightId, TargetEmployeeId));
    }

    [Test]
    public async Task AssignEmployeeToFlightUsingIdsAsync_ArgumentsAndScheduleAreValid_CallsRepository()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var validFlight = new Flight { Id = TargetFlightId, Date = DateTime.Today, Route = new Route { Id = TargetRouteId } };
        var validRoute = new Route
        {
            DepartureTime = TimeOnly.FromDateTime(DateTime.Today),
            ArrivalTime = TimeOnly.FromDateTime(DateTime.Today.AddHours(1))
        };

        employeeRepository.GetByIdAsync(TargetEmployeeId).Returns(Task.FromResult<Employee?>(new Employee()));
        flightRepository.GetByIdAsync(TargetFlightId).Returns(Task.FromResult<Flight?>(validFlight));
        employeeFlightRepository.GetEmployeeIdsByFlightIdAsync(TargetFlightId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int>()));
        routeRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Route?>(validRoute));
        employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(TargetEmployeeId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int>()));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            employeeRepository: employeeRepository,
            flightRepository: flightRepository,
            routeRepository: routeRepository);

        await service.AssignEmployeeToFlightUsingIdsAsync(TargetFlightId, TargetEmployeeId);

        await employeeFlightRepository.Received(1).AssignAsync(TargetEmployeeId, TargetFlightId);
    }

    [Test]
    public async Task RemoveEmployeeFromFlightUsingIdsAsync_WithCorrectIds_CallsRepository()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        employeeFlightRepository.UnassignAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(Task.CompletedTask);

        var service = CreateTestService(employeeFlightRepository: employeeFlightRepository);
        await service.RemoveEmployeeFromFlightUsingIdsAsync(TargetFlightId, TargetEmployeeId);

        await employeeFlightRepository.Received(1).UnassignAsync(TargetEmployeeId, TargetFlightId);
    }

    [Test]
    public async Task RemoveAllCrewAssignmentsForFlightAsync_IdIsValid_CallsRepository()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        employeeFlightRepository.DeleteByFlightIdAsync(TargetFlightId).Returns(Task.CompletedTask);

        var service = CreateTestService(employeeFlightRepository: employeeFlightRepository);
        await service.RemoveAllCrewAssignmentsForFlightAsync(TargetFlightId);

        await employeeFlightRepository.Received(1).DeleteByFlightIdAsync(TargetFlightId);
    }

    [Test]
    public async Task RemoveAllCrewAssignmentsForFlightAsync_IdIsInvalid_DoesNotCallRepository()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();

        var service = CreateTestService(employeeFlightRepository: employeeFlightRepository);
        await service.RemoveAllCrewAssignmentsForFlightAsync(InvalidId);

        await employeeFlightRepository.DidNotReceive().DeleteByFlightIdAsync(Arg.Any<int>());
    }

    [Test]
    public async Task RemoveAllFlightsAssignmentsForEmployeeAsync_IdIsValid_CallsRepository()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        employeeFlightRepository.DeleteByEmployeeIdAsync(TargetEmployeeId).Returns(Task.CompletedTask);

        var service = CreateTestService(employeeFlightRepository: employeeFlightRepository);
        await service.RemoveAllFlightsAssignmentsForEmployeeAsync(TargetEmployeeId);

        await employeeFlightRepository.Received(1).DeleteByEmployeeIdAsync(TargetEmployeeId);
    }

    [Test]
    public async Task RemoveAllFlightsAssignmentsForEmployeeAsync_IdIsInvalid_DoesNotCallRepository()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();

        var service = CreateTestService(employeeFlightRepository: employeeFlightRepository);
        await service.RemoveAllFlightsAssignmentsForEmployeeAsync(InvalidId);

        await employeeFlightRepository.DidNotReceive().DeleteByEmployeeIdAsync(Arg.Any<int>());
    }

    [Test]
    public async Task GetEmployeesAssignedToFlightAsync_SomeIdsAreNotFound_ReturnsOnlyExistingEmployees()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();

        employeeFlightRepository.GetEmployeeIdsByFlightIdAsync(TargetFlightId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int> { TargetEmployeeId, MissingEmployeeId }));
        employeeRepository.GetByIdAsync(TargetEmployeeId)
            .Returns(Task.FromResult<Employee?>(new Employee { Id = TargetEmployeeId }));
        employeeRepository.GetByIdAsync(MissingEmployeeId)
            .Returns(Task.FromResult<Employee?>(null));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            employeeRepository: employeeRepository);

        var result = (await service.GetEmployeesAssignedToFlightAsync(TargetFlightId)).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(TargetEmployeeId));
    }

    [Test]
    public async Task GetEmployeeScheduleAsync_EmployeeIdIsInvalid_ReturnsEmptyList()
    {
        var service = CreateTestService();

        var result = await service.GetEmployeeScheduleAsync(InvalidId);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetEmployeeScheduleAsync_RepositoryReturnsNullForId_SkipsNullFlights()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();

        employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(TargetEmployeeId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int> { TargetFlightId, MissingFlightId }));
        flightRepository.GetByIdAsync(TargetFlightId)
            .Returns(Task.FromResult<Flight?>(new Flight { Id = TargetFlightId, Route = new Route { Id = TargetRouteId } }));
        flightRepository.GetByIdAsync(MissingFlightId)
            .Returns(Task.FromResult<Flight?>(null));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            flightRepository: flightRepository);

        var result = (await service.GetEmployeeScheduleAsync(TargetEmployeeId)).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(TargetFlightId));
    }

    [Test]
    public async Task IsEmployeeAvailableAsync_TargetRouteIsNotFound_ReturnsFalse()
    {
        var routeRepository = Substitute.For<IRouteRepository>();
        routeRepository.GetByIdAsync(TargetRouteId).Returns(Task.FromResult<Route?>(null));

        var service = CreateTestService(routeRepository: routeRepository);
        var result = await service.IsEmployeeAvailableAsync(TargetEmployeeId, DateTime.Today, TargetRouteId);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsEmployeeAvailableAsync_NoFlightsOverlap_ReturnsTrue()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var validRoute = new Route
        {
            Id = TargetRouteId,
            DepartureTime = TimeOnly.FromDateTime(DateTime.Today),
            ArrivalTime = TimeOnly.FromDateTime(DateTime.Today.AddHours(1))
        };

        routeRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Route?>(validRoute));
        employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(TargetEmployeeId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int>()));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            routeRepository: routeRepository);

        var result = await service.IsEmployeeAvailableAsync(TargetEmployeeId, DateTime.Today, TargetRouteId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsEmployeeAvailableAsync_FlightTimesOverlap_ReturnsFalse()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var targetRoute = new Route
        {
            Id = TargetRouteId,
            DepartureTime = new TimeOnly(TargetDepartureHour, 0),
            ArrivalTime = new TimeOnly(TargetArrivalHour, 0)
        };
        var conflictingRoute = new Route
        {
            Id = ConflictingRouteId,
            DepartureTime = new TimeOnly(ConflictingDepartureHour, 0),
            ArrivalTime = new TimeOnly(ConflictingArrivalHour, 0)
        };

        routeRepository.GetByIdAsync(TargetRouteId).Returns(Task.FromResult<Route?>(targetRoute));
        routeRepository.GetByIdAsync(ConflictingRouteId).Returns(Task.FromResult<Route?>(conflictingRoute));
        employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(TargetEmployeeId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int> { ConflictingFlightId }));
        flightRepository.GetByIdAsync(ConflictingFlightId)
            .Returns(Task.FromResult<Flight?>(new Flight { Id = ConflictingFlightId, Date = DateTime.Today, Route = new Route { Id = ConflictingRouteId } }));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            flightRepository: flightRepository,
            routeRepository: routeRepository);

        var result = await service.IsEmployeeAvailableAsync(TargetEmployeeId, DateTime.Today, TargetRouteId);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsEmployeeAvailableAsync_ConflictMatchesExcludedFlightId_ReturnsTrue()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var existingFlight = new Flight { Id = ConflictingFlightId, Date = DateTime.Today, Route = new Route { Id = TargetRouteId } };
        var targetRoute = new Route { Id = TargetRouteId };

        employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(TargetEmployeeId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int> { ConflictingFlightId }));
        flightRepository.GetByIdAsync(ConflictingFlightId).Returns(Task.FromResult<Flight?>(existingFlight));
        routeRepository.GetByIdAsync(TargetRouteId).Returns(Task.FromResult<Route?>(targetRoute));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            flightRepository: flightRepository,
            routeRepository: routeRepository);

        var result = await service.IsEmployeeAvailableAsync(TargetEmployeeId, DateTime.Today, TargetRouteId, excludedFlightId: ConflictingFlightId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsEmployeeAvailableAsync_ExistingFlightIsOnDifferentDate_ReturnsTrue()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var targetRoute = new Route
        {
            Id = TargetRouteId,
            DepartureTime = new TimeOnly(TargetDepartureHour, 0),
            ArrivalTime = new TimeOnly(TargetArrivalHour, 0)
        };
        var nextDayFlight = new Flight { Id = NextDayFlightId, Date = DateTime.Today.AddDays(1), Route = new Route { Id = ConflictingRouteId } };

        routeRepository.GetByIdAsync(TargetRouteId).Returns(Task.FromResult<Route?>(targetRoute));
        employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(TargetEmployeeId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int> { NextDayFlightId }));
        flightRepository.GetByIdAsync(NextDayFlightId).Returns(Task.FromResult<Flight?>(nextDayFlight));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            flightRepository: flightRepository,
            routeRepository: routeRepository);

        var result = await service.IsEmployeeAvailableAsync(TargetEmployeeId, DateTime.Today, TargetRouteId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsEmployeeAvailableAsync_ExistingFlightRouteIsNull_ReturnsTrue()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var targetRoute = new Route
        {
            Id = TargetRouteId,
            DepartureTime = new TimeOnly(TargetDepartureHour, 0),
            ArrivalTime = new TimeOnly(TargetArrivalHour, 0)
        };
        var flightWithMissingRoute = new Flight { Id = TargetFlightId, Date = DateTime.Today, Route = new Route { Id = MissingRouteId } };

        routeRepository.GetByIdAsync(TargetRouteId).Returns(Task.FromResult<Route?>(targetRoute));
        routeRepository.GetByIdAsync(MissingRouteId).Returns(Task.FromResult<Route?>(null));
        employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(TargetEmployeeId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int> { TargetFlightId }));
        flightRepository.GetByIdAsync(TargetFlightId).Returns(Task.FromResult<Flight?>(flightWithMissingRoute));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            flightRepository: flightRepository,
            routeRepository: routeRepository);

        var result = await service.IsEmployeeAvailableAsync(TargetEmployeeId, DateTime.Today, TargetRouteId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task AssignEmpolyeesToFlightUsingIdsAsync_PartialFailureOccurs_ContinuesProcessing()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();

        employeeRepository.GetByIdAsync(FailingEmployeeId).ThrowsAsync(new Exception("Simulated DB Failure"));
        employeeRepository.GetByIdAsync(SucceedingEmployeeId).Returns(Task.FromResult<Employee?>(new Employee()));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            employeeRepository: employeeRepository);

        await service.AssignEmpolyeesToFlightUsingIdsAsync(TargetFlightId, new List<int> { FailingEmployeeId, SucceedingEmployeeId });

        await employeeFlightRepository.DidNotReceive().AssignAsync(SucceedingEmployeeId, TargetFlightId);
    }

    [Test]
    public async Task UpdateEmployeesForFlightUsingIdsAsync_WithUpdatedCrewIds_AddsNewAndRemovesMissingEmployees()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var existingCrewIds = new List<int> { EmployeeToRemoveId, EmployeeToKeepId };
        var updatedCrewIds = new List<int> { EmployeeToKeepId, EmployeeToAddId };

        employeeFlightRepository.GetEmployeeIdsByFlightIdAsync(TargetFlightId)
            .Returns(Task.FromResult<IEnumerable<int>>(existingCrewIds));
        employeeFlightRepository.UnassignAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(Task.CompletedTask);
        employeeRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Employee?>(new Employee()));
        flightRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Flight?>(
            new Flight { Id = TargetFlightId, Route = new Route { Id = TargetRouteId }, Date = DateTime.Today }));
        routeRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Route?>(new Route()));
        employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(Arg.Any<int>())
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int>()));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            employeeRepository: employeeRepository,
            flightRepository: flightRepository,
            routeRepository: routeRepository);

        await service.UpdateEmployeesForFlightUsingIdsAsync(TargetFlightId, updatedCrewIds);

        await employeeFlightRepository.Received(1).UnassignAsync(EmployeeToRemoveId, TargetFlightId);
        await employeeFlightRepository.Received(1).AssignAsync(EmployeeToAddId, TargetFlightId);
    }

    [Test]
    public async Task GetFormattedEmployeeScheduleAsync_IdIsInvalid_ReturnsEmptyList()
    {
        var service = CreateTestService();

        var result = await service.GetFormattedEmployeeScheduleAsync(InvalidId);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetFormattedEmployeeScheduleAsync_GateAndRunwayAreNull_UsesDefaultPlaceholders()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var runwayService = Substitute.For<IRunwayService>();
        var routeService = Substitute.For<IRouteService>();

        var incompleteFlight = new Flight
        {
            Id = TargetFlightId,
            FlightNumber = DefaultFlightCode,
            Date = DateTime.Today,
            Route = new Route { Id = TargetRouteId },
            Gate = null!,
            Runway = null!
        };

        employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(TargetEmployeeId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int> { TargetFlightId }));
        flightRepository.GetByIdAsync(TargetFlightId).Returns(Task.FromResult<Flight?>(incompleteFlight));
        routeRepository.GetByIdAsync(TargetRouteId).Returns(Task.FromResult<Route?>(incompleteFlight.Route));
        routeService.NormalizeFlightType(Arg.Any<string>()).Returns("DEP");
        routeService.GetRelevantTime(Arg.Any<Route>()).Returns("10:00");

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            flightRepository: flightRepository,
            routeRepository: routeRepository,
            runwayService: runwayService,
            routeService: routeService);

        var result = (await service.GetFormattedEmployeeScheduleAsync(TargetEmployeeId)).ToList();

        Assert.That(result[0].GateName, Is.EqualTo(PlaceholderDash));
        Assert.That(result[0].RunwayName, Is.EqualTo(PlaceholderDash));
    }

    [Test]
    public async Task GetFormattedEmployeeScheduleAsync_WithMultipleScheduledFlights_SortsResultsByDate()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var runwayService = Substitute.For<IRunwayService>();
        var routeService = Substitute.For<IRouteService>();

        var futureFlight = new Flight { Id = FutureFlightId, Date = DateTime.Today.AddDays(1), Route = new Route { Id = TargetRouteId } };
        var currentFlight = new Flight { Id = CurrentFlightId, Date = DateTime.Today, Route = new Route { Id = TargetRouteId } };

        employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(TargetEmployeeId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int> { FutureFlightId, CurrentFlightId }));
        flightRepository.GetByIdAsync(FutureFlightId).Returns(Task.FromResult<Flight?>(futureFlight));
        flightRepository.GetByIdAsync(CurrentFlightId).Returns(Task.FromResult<Flight?>(currentFlight));
        routeRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Route?>(new Route()));
        routeService.NormalizeFlightType(Arg.Any<string>()).Returns(FutureFlightType);
        routeService.GetRelevantTime(Arg.Any<Route>()).Returns(FutureFlightType);
        runwayService.GetRunwayByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Runway?>(null));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            flightRepository: flightRepository,
            routeRepository: routeRepository,
            runwayService: runwayService,
            routeService: routeService);

        var sortedResults = (await service.GetFormattedEmployeeScheduleAsync(TargetEmployeeId)).ToList();

        Assert.That(sortedResults[0].Id, Is.EqualTo(CurrentFlightId.ToString()));
        Assert.That(sortedResults[1].Id, Is.EqualTo(FutureFlightId.ToString()));
    }

    [Test]
    public async Task GetAvailableEmployeesGroupedByRoleAsync_WithUnavailableEmployees_FiltersOutUnavailableEmployees()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var targetFlight = new Flight { Id = TargetFlightId, Date = DateTime.Today, Route = new Route { Id = TargetRouteId } };
        var allEmployees = new List<Employee>
        {
            new Employee { Id = AvailableEmployeeId },
            new Employee { Id = UnavailableEmployeeId }
        };
        var targetRoute = new Route
        {
            Id = TargetRouteId,
            DepartureTime = new TimeOnly(TargetDepartureHour, 0),
            ArrivalTime = new TimeOnly(TargetArrivalHour, 0)
        };
        var conflictingRoute = new Route
        {
            Id = ConflictingRouteId,
            DepartureTime = new TimeOnly(ConflictingDepartureHour, 0),
            ArrivalTime = new TimeOnly(ConflictingArrivalHour, 0)
        };
        var conflictingFlight = new Flight { Id = ConflictingFlightId, Date = DateTime.Today, Route = new Route { Id = ConflictingRouteId } };

        employeeRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Employee>>(allEmployees));
        routeRepository.GetByIdAsync(TargetRouteId).Returns(Task.FromResult<Route?>(targetRoute));
        employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(AvailableEmployeeId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int>()));
        employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(UnavailableEmployeeId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int> { ConflictingFlightId }));
        flightRepository.GetByIdAsync(ConflictingFlightId).Returns(Task.FromResult<Flight?>(conflictingFlight));
        routeRepository.GetByIdAsync(ConflictingRouteId).Returns(Task.FromResult<Route?>(conflictingRoute));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            employeeRepository: employeeRepository,
            flightRepository: flightRepository,
            routeRepository: routeRepository);

        var result = (await service.GetAvailableEmployeesGroupedByRoleAsync(targetFlight)).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(AvailableEmployeeId));
    }

    [Test]
    public async Task GetAvailableEmployeesGroupedByRoleAsync_WithUnsortedEmployees_SortsByRoleThenName()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var targetFlight = new Flight { Id = TargetFlightId, Date = DateTime.Today, Route = new Route { Id = TargetRouteId } };
        var unsortedEmployees = new List<Employee>
        {
            new Employee { Id = CoPilotBobId,   Name = CoPilotBobName,   Role = EmployeeRoleEnum.CoPilot },
            new Employee { Id = PilotAliceId,   Name = PilotAliceName,   Role = EmployeeRoleEnum.Pilot },
            new Employee { Id = PilotCharlieId, Name = PilotCharlieName, Role = EmployeeRoleEnum.Pilot }
        };

        employeeRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Employee>>(unsortedEmployees));
        routeRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Route?>(new Route()));
        employeeFlightRepository.GetFlightIdsByEmployeeIdAsync(Arg.Any<int>())
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int>()));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            employeeRepository: employeeRepository,
            routeRepository: routeRepository);

        var sortedResult = (await service.GetAvailableEmployeesGroupedByRoleAsync(targetFlight)).ToList();

        Assert.That(sortedResult.Count, Is.EqualTo(3));
        Assert.That(sortedResult[0].Id, Is.EqualTo(PilotAliceId));
        Assert.That(sortedResult[1].Id, Is.EqualTo(PilotCharlieId));
        Assert.That(sortedResult[2].Id, Is.EqualTo(CoPilotBobId));
    }

    [Test]
    public void FormatCrewList_NoCrewIsAssigned_ReturnsUnassigned()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        employeeFlightRepository.GetEmployeeIdsByFlightIdAsync(TargetFlightId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int>()));

        var service = CreateTestService(employeeFlightRepository: employeeFlightRepository);

        var result = service.FormatCrewList(TargetFlightId);

        Assert.That(result, Is.EqualTo("Unassigned"));
    }

    [Test]
    public void FormatCrewList_CrewIsAssigned_ReturnsFormattedNames()
    {
        var employeeFlightRepository = Substitute.For<IEmployeeFlightRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();

        employeeFlightRepository.GetEmployeeIdsByFlightIdAsync(TargetFlightId)
            .Returns(Task.FromResult<IEnumerable<int>>(new List<int> { PilotAliceId, CoPilotBobId }));
        employeeRepository.GetByIdAsync(PilotAliceId)
            .Returns(Task.FromResult<Employee?>(new Employee { Id = PilotAliceId, Name = PilotAliceName }));
        employeeRepository.GetByIdAsync(CoPilotBobId)
            .Returns(Task.FromResult<Employee?>(new Employee { Id = CoPilotBobId, Name = CoPilotBobName }));

        var service = CreateTestService(
            employeeFlightRepository: employeeFlightRepository,
            employeeRepository: employeeRepository);

        var result = service.FormatCrewList(TargetFlightId);

        Assert.That(result, Does.Contain(PilotAliceName));
        Assert.That(result, Does.Contain(CoPilotBobName));
    }
}
