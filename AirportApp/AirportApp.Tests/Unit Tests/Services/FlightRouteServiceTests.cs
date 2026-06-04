using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using AirportApp.ClassLibrary.Service.Interface;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class FlightRouteServiceTests
{
    private const int TargetCompanyId = 1;
    private const int TargetAirportId = 1;
    private const int TargetFlightId = 1;
    private const int TargetRouteId = 5;
    private const int TargetRunwayId = 1;
    private const int TargetGateId = 1;
    private const int ConflictingRunwayId = 99;
    private const int ConflictingGateId = 99;
    private const int ConflictingRouteId = 99;
    private const int GeneratedRouteId = 20;
    private const int ValidCapacity = 100;
    private const int InvalidCapacity = 0;
    private const int ValidInterval = 1;
    private const int MissingRouteId = 999;
    private const int ExpectedRouteId = 8;
    private const int NonRecurrentInterval = 0;
    private const int OtherCompanyId = 99;
    private const int EmptyForeignKeyId = 0;
    private const int InvalidCompanyId = 0;
    private const int InvalidAirportId = 0;

    private const string RouteTypeDeparture = "DEP";
    private const string ValidFlightNumber = "FL001";
    private const string ConflictingFlightNumber = "EX001";
    private const string NullFlightNumber = "NULLROUTE";
    private const string CustomRecurrenceType = "Custom";
    private const string DailyRecurrenceType = "Daily";
    private const string WeeklyRecurrenceType = "Weekly";
    private const string MonthlyRecurrenceType = "Monthly";
    private const string ValidCustomDaysInterval = "5";
    private const string InvalidCustomDaysInterval = "abc";
    private const string InvalidZeroDaysInterval = "0";
    private const string InvalidBiweeklyRecurrenceType = "Biweekly";
    private const string TargetAirportCode = "LHR";
    private const string TargetRunwayName = "Runway 2";
    private const string TargetGateName = "Gate 3";
    private const string MissingDestinationPlaceholder = "-";
    private const string AirportCodeJfk = "JFK";
    private const string AirportNameJfk = "John F. Kennedy";
    private const string ExpectedDestinationTextJfk = "JFK - John F. Kennedy";
    private const string ValidCrewText = "Crew A";

    private static readonly DateTime TargetStartDate = new DateTime(2026, 6, 10);
    private static readonly DateTime TargetEndDate = new DateTime(2026, 6, 20);
    private static readonly DateTime InvalidPastDate = new DateTime(2026, 6, 1);

    private static readonly TimeOnly TargetDepartureTime = new TimeOnly(10, 0);
    private static readonly TimeOnly TargetArrivalTime = new TimeOnly(12, 0);
    private static readonly TimeOnly ConflictingDepartureTime = new TimeOnly(10, 30);
    private static readonly TimeOnly ConflictingArrivalTime = new TimeOnly(11, 30);

    private static readonly TimeOnly BeforeMidnightDepartureTimeExisting = new TimeOnly(23, 0);
    private static readonly TimeOnly AfterMidnightArrivalTimeExisting = new TimeOnly(1, 0);
    private static readonly TimeOnly BeforeMidnightDepartureTimeTarget = new TimeOnly(22, 30);
    private static readonly TimeOnly AfterMidnightArrivalTimeTarget = new TimeOnly(0, 30);

    private static FlightRouteService CreateTestService(
        IFlightRepository flightRepository,
        IRouteRepository routeRepository,
        ICompanyRepository? companyRepository = null,
        IAirportRepository? airportRepository = null,
        IRunwayService? runwayService = null,
        IGateService? gateService = null,
        IAirportService? airportService = null)
    {
        return new FlightRouteService(
            flightRepository,
            routeRepository,
            companyRepository ?? Substitute.For<ICompanyRepository>(),
            airportRepository ?? Substitute.For<IAirportRepository>(),
            runwayService ?? Substitute.For<IRunwayService>(),
            gateService ?? Substitute.For<IGateService>(),
            airportService ?? Substitute.For<IAirportService>());
    }

    private static (
        IFlightRepository flightRepository,
        IRouteRepository routeRepository,
        ICompanyRepository companyRepository,
        IAirportRepository airportRepository) ConfigureSuccessfulRepositories()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var companyRepository = Substitute.For<ICompanyRepository>();
        var airportRepository = Substitute.For<IAirportRepository>();

        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));
        flightRepository.AddAsync(Arg.Any<Flight>()).Returns(Task.FromResult(TargetFlightId));
        companyRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Company?>(new Company()));
        airportRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Airport?>(new Airport()));
        routeRepository.AddAsync(Arg.Any<Route>()).Returns(Task.FromResult(TargetRouteId));

        return (flightRepository, routeRepository, companyRepository, airportRepository);
    }

    [Test]
    public void AddFlightToRouteAsync_StartDateIsAfterEndDate_ThrowsArgumentException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            flightRouteService.AddFlightToRouteAsync(TargetCompanyId, TargetAirportId, RouteTypeDeparture, ValidInterval,
                TargetStartDate, InvalidPastDate, TargetDepartureTime, TargetArrivalTime, ValidCapacity, ValidFlightNumber, TargetRunwayId, TargetGateId));
    }

    [Test]
    public async Task AddFlightToRouteAsync_CapacityIsZero_ThrowsArgumentException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        var exception = Assert.ThrowsAsync<ArgumentException>(() =>
            flightRouteService.AddFlightToRouteAsync(TargetCompanyId, TargetAirportId, RouteTypeDeparture, ValidInterval,
                TargetStartDate, TargetEndDate, TargetDepartureTime, TargetArrivalTime, InvalidCapacity, ValidFlightNumber, TargetRunwayId, TargetGateId));

        Assert.That(exception!.Message, Does.Contain("Capacity must be a positive number greater than 0."));
    }

    [Test]
    public void AddFlightToRouteAsync_GateIsOccupied_ThrowsInvalidOperationException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var existingFlight = new Flight
        {
            Gate = new Gate { Id = TargetGateId },
            Runway = new Runway { Id = ConflictingRunwayId },
            Route = new Route { Id = TargetRouteId },
            FlightNumber = ConflictingFlightNumber,
            Date = TargetStartDate
        };
        var existingRoute = new Route
        {
            DepartureTime = ConflictingDepartureTime,
            ArrivalTime = ConflictingArrivalTime
        };

        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { existingFlight }));
        routeRepository.GetByIdAsync(TargetRouteId).Returns(Task.FromResult<Route?>(existingRoute));

        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            flightRouteService.AddFlightToRouteAsync(TargetCompanyId, TargetAirportId, RouteTypeDeparture, ValidInterval,
                TargetStartDate, TargetStartDate, TargetDepartureTime, TargetArrivalTime, ValidCapacity, ValidFlightNumber, TargetRunwayId, TargetGateId));
    }

    [Test]
    public void AddFlightToRouteAsync_RunwayIsOccupied_ThrowsInvalidOperationException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var existingFlight = new Flight
        {
            Gate = new Gate { Id = ConflictingGateId },
            Runway = new Runway { Id = TargetRunwayId },
            Route = new Route { Id = TargetRouteId },
            FlightNumber = ConflictingFlightNumber,
            Date = TargetStartDate
        };
        var existingRoute = new Route
        {
            DepartureTime = ConflictingDepartureTime,
            ArrivalTime = ConflictingArrivalTime
        };

        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { existingFlight }));
        routeRepository.GetByIdAsync(TargetRouteId).Returns(Task.FromResult<Route?>(existingRoute));

        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            flightRouteService.AddFlightToRouteAsync(TargetCompanyId, TargetAirportId, RouteTypeDeparture, ValidInterval,
                TargetStartDate, TargetStartDate, TargetDepartureTime, TargetArrivalTime, ValidCapacity, ValidFlightNumber, TargetRunwayId, TargetGateId));
    }

    [Test]
    public async Task AddFlightToRouteAsync_NoConflictsExist_ReturnsGeneratedRouteId()
    {
        var (flightRepository, routeRepository, companyRepository, airportRepository) = ConfigureSuccessfulRepositories();
        routeRepository.AddAsync(Arg.Any<Route>()).Returns(Task.FromResult(GeneratedRouteId));

        var flightRouteService = CreateTestService(flightRepository, routeRepository, companyRepository, airportRepository);

        int resultId = await flightRouteService.AddFlightToRouteAsync(TargetCompanyId, TargetAirportId, RouteTypeDeparture, ValidInterval,
            TargetStartDate, TargetStartDate, TargetDepartureTime, TargetArrivalTime, ValidCapacity, ValidFlightNumber, TargetRunwayId, TargetGateId);

        Assert.That(resultId, Is.EqualTo(GeneratedRouteId));
        await routeRepository.Received(1).AddAsync(Arg.Any<Route>());
        await flightRepository.Received(1).AddAsync(Arg.Any<Flight>());
    }

    [Test]
    public async Task AddFlightToRouteAsync_ExistingFlightIsOnDifferentDate_DoesNotCheckRoute()
    {
        var (flightRepository, routeRepository, companyRepository, airportRepository) = ConfigureSuccessfulRepositories();

        var otherDayFlight = new Flight
        {
            Date = TargetStartDate.AddDays(1),
            Gate = new Gate { Id = TargetGateId },
            Runway = new Runway { Id = TargetRunwayId },
            Route = new Route { Id = TargetRouteId },
            FlightNumber = ConflictingFlightNumber
        };

        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { otherDayFlight }));
        routeRepository.AddAsync(Arg.Any<Route>()).Returns(Task.FromResult(GeneratedRouteId));

        var flightRouteService = CreateTestService(flightRepository, routeRepository, companyRepository, airportRepository);

        int resultId = await flightRouteService.AddFlightToRouteAsync(TargetCompanyId, TargetAirportId, RouteTypeDeparture, ValidInterval,
            TargetStartDate, TargetStartDate, TargetDepartureTime, TargetArrivalTime, ValidCapacity, ValidFlightNumber, TargetRunwayId, TargetGateId);

        Assert.That(resultId, Is.EqualTo(GeneratedRouteId));
        await routeRepository.DidNotReceive().GetByIdAsync(TargetRouteId);
    }

    [Test]
    public void AddFlightToRouteAsync_TimesOverlapAcrossMidnight_ThrowsInvalidOperationException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var existingFlight = new Flight
        {
            Date = TargetStartDate,
            Route = new Route { Id = TargetRouteId },
            Gate = new Gate { Id = TargetGateId },
            Runway = new Runway { Id = ConflictingRunwayId }
        };
        var routeCrossingMidnight = new Route
        {
            DepartureTime = BeforeMidnightDepartureTimeExisting,
            ArrivalTime = AfterMidnightArrivalTimeExisting
        };

        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { existingFlight }));
        routeRepository.GetByIdAsync(TargetRouteId).Returns(Task.FromResult<Route?>(routeCrossingMidnight));

        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            flightRouteService.AddFlightToRouteAsync(TargetCompanyId, TargetAirportId, RouteTypeDeparture, ValidInterval,
                TargetStartDate, TargetStartDate, BeforeMidnightDepartureTimeTarget, AfterMidnightArrivalTimeTarget, ValidCapacity, ValidFlightNumber, ConflictingRunwayId, TargetGateId));
    }

    [Test]
    public async Task AddFlightToRouteAsync_ExistingRouteIsNull_ReturnsGeneratedRouteId()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var companyRepository = Substitute.For<ICompanyRepository>();
        var airportRepository = Substitute.For<IAirportRepository>();

        var flightWithMissingRoute = new Flight
        {
            Date = TargetStartDate,
            Gate = new Gate { Id = TargetGateId },
            Runway = new Runway { Id = TargetRunwayId },
            Route = new Route { Id = MissingRouteId },
            FlightNumber = NullFlightNumber
        };

        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { flightWithMissingRoute }));
        flightRepository.AddAsync(Arg.Any<Flight>()).Returns(Task.FromResult(TargetFlightId));
        routeRepository.GetByIdAsync(MissingRouteId).Returns(Task.FromResult<Route?>(null));
        companyRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Company?>(new Company()));
        airportRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Airport?>(new Airport()));
        routeRepository.AddAsync(Arg.Any<Route>()).Returns(Task.FromResult(ExpectedRouteId));

        var flightRouteService = CreateTestService(
            flightRepository: flightRepository,
            routeRepository: routeRepository,
            companyRepository: companyRepository,
            airportRepository: airportRepository);

        int resultId = await flightRouteService.AddFlightToRouteAsync(
            TargetCompanyId, TargetAirportId, RouteTypeDeparture, NonRecurrentInterval,
            TargetStartDate, TargetStartDate, TargetDepartureTime, TargetArrivalTime,
            ValidCapacity, ValidFlightNumber, TargetRunwayId, TargetGateId);

        Assert.That(resultId, Is.EqualTo(ExpectedRouteId));
    }

    [Test]
    public void DeleteFlightUsingIdAsync_IdIsNegative_ThrowsArgumentException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        Assert.ThrowsAsync<ArgumentException>(() => flightRouteService.DeleteFlightUsingIdAsync(-1));
    }

    [Test]
    public void DeleteFlightUsingIdAsync_FlightIsNotFound_ThrowsArgumentException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        flightRepository.GetByIdAsync(TargetFlightId).Returns(Task.FromResult<Flight?>(null));

        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        Assert.ThrowsAsync<ArgumentException>(() => flightRouteService.DeleteFlightUsingIdAsync(TargetFlightId));
    }

    [Test]
    public async Task DeleteFlightUsingIdAsync_IdIsValid_CallsRepositoryDelete()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        flightRepository.GetByIdAsync(TargetFlightId).Returns(Task.FromResult<Flight?>(new Flight()));
        flightRepository.DeleteAsync(TargetFlightId).Returns(Task.CompletedTask);

        var flightRouteService = CreateTestService(flightRepository, routeRepository);
        await flightRouteService.DeleteFlightUsingIdAsync(TargetFlightId);

        await flightRepository.Received(1).DeleteAsync(TargetFlightId);
    }

    [Test]
    public async Task GetFlightsByCompanyIdAsync_RoutesMatchCompany_ReturnsFilteredFlights()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var routesList = new List<Route>
        {
            new Route { Id = TargetRouteId, Company = new Company { Id = TargetCompanyId } },
            new Route { Id = ConflictingRouteId, Company = new Company { Id = OtherCompanyId } }
        };
        var flightsList = new List<Flight>
        {
            new Flight { FlightNumber = ValidFlightNumber, Route = new Route { Id = TargetRouteId } },
            new Flight { FlightNumber = ConflictingFlightNumber, Route = new Route { Id = ConflictingRouteId } }
        };

        routeRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Route>>(routesList));
        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(flightsList));

        var flightRouteService = CreateTestService(flightRepository, routeRepository);
        var resultList = (await flightRouteService.GetFlightsByCompanyIdAsync(TargetCompanyId)).ToList();

        Assert.That(resultList.Count, Is.EqualTo(1));
        Assert.That(resultList[0].FlightNumber, Is.EqualTo(ValidFlightNumber));
    }

    [Test]
    public async Task GetFlightsByCompanyIdAsync_NoRoutesMatchCompany_ReturnsEmptyList()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        routeRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Route>>(new List<Route>()));
        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(
            new List<Flight> { new Flight { Route = new Route { Id = TargetRouteId } } }));

        var flightRouteService = CreateTestService(flightRepository, routeRepository);
        var resultList = (await flightRouteService.GetFlightsByCompanyIdAsync(TargetCompanyId)).ToList();

        Assert.That(resultList, Is.Empty);
    }

    [Test]
    public void GetDestinationText_RouteIsNull_ReturnsPlaceholder()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        var flightWithNoRoute = new Flight { Route = null! };
        string resultText = flightRouteService.GetDestinationText(flightWithNoRoute);

        Assert.That(resultText, Is.EqualTo(MissingDestinationPlaceholder));
    }

    [Test]
    public void GetDestinationText_AirportIsNull_ReturnsPlaceholder()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        var flightWithNoAirport = new Flight { Route = new Route { Airport = null! } };
        string resultText = flightRouteService.GetDestinationText(flightWithNoAirport);

        Assert.That(resultText, Is.EqualTo(MissingDestinationPlaceholder));
    }

    [Test]
    public void GetDestinationText_AirportIsValid_ReturnsFormattedString()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        var validFlight = new Flight
        {
            Route = new Route
            {
                Airport = new Airport { AirportCode = AirportCodeJfk, Name = AirportNameJfk }
            }
        };
        string resultText = flightRouteService.GetDestinationText(validFlight);

        Assert.That(resultText, Is.EqualTo(ExpectedDestinationTextJfk));
    }

    [Test]
    public async Task GetAllFlightsWithDetailsAsync_ForeignKeysAreNullOrInvalid_SkipsHydration()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();

        var flightWithMissingForeignKeys = new Flight
        {
            Runway = new Runway { Id = EmptyForeignKeyId },
            Gate = null!,
            Route = new Route { Id = TargetRouteId, Airport = new Airport { Id = EmptyForeignKeyId } }
        };

        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { flightWithMissingForeignKeys }));
        routeRepository.GetByIdAsync(TargetRouteId).Returns(Task.FromResult<Route?>(new Route { Airport = new Airport { Id = EmptyForeignKeyId } }));

        var flightRouteService = CreateTestService(flightRepository, routeRepository);
        var resultList = (await flightRouteService.GetAllFlightsWithDetailsAsync()).ToList();

        Assert.That(resultList.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetAllFlightsWithDetailsAsync_ForeignKeysAreValid_HydratesAllProperties()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var runwayService = Substitute.For<IRunwayService>();
        var gateService = Substitute.For<IGateService>();
        var airportService = Substitute.For<IAirportService>();

        var targetAirport = new Airport { Id = TargetAirportId, AirportCode = TargetAirportCode };
        var targetRoute = new Route { Id = TargetRouteId, Airport = new Airport { Id = TargetAirportId } };
        var targetRunway = new Runway { Id = TargetRunwayId, Name = TargetRunwayName };
        var targetGate = new Gate { Id = TargetGateId, GateName = TargetGateName };

        var targetFlight = new Flight
        {
            Runway = new Runway { Id = TargetRunwayId },
            Gate = new Gate { Id = TargetGateId },
            Route = new Route { Id = TargetRouteId }
        };

        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { targetFlight }));
        routeRepository.GetByIdAsync(TargetRouteId).Returns(Task.FromResult<Route?>(targetRoute));
        runwayService.GetRunwayByIdAsync(TargetRunwayId).Returns(Task.FromResult<Runway?>(targetRunway));
        gateService.GetGateByIdAsync(TargetGateId).Returns(Task.FromResult<Gate?>(targetGate));
        airportService.GetAirportByIdAsync(TargetAirportId).Returns(Task.FromResult<Airport?>(targetAirport));

        var flightRouteService = CreateTestService(flightRepository, routeRepository,
            runwayService: runwayService, gateService: gateService, airportService: airportService);

        var resultList = (await flightRouteService.GetAllFlightsWithDetailsAsync()).ToList();

        Assert.That(resultList.Count, Is.EqualTo(1));
        Assert.That(resultList[0].Runway.Name, Is.EqualTo(TargetRunwayName));
        Assert.That(resultList[0].Gate.GateName, Is.EqualTo(TargetGateName));
        Assert.That(resultList[0].Route.Airport.AirportCode, Is.EqualTo(TargetAirportCode));
    }

    [Test]
    public void CreateFlightWithScheduleAsync_CompanyIdIsInvalid_ThrowsInvalidOperationException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            flightRouteService.CreateFlightWithScheduleAsync(InvalidCompanyId, RouteTypeDeparture, TargetAirportId, ValidCapacity,
                TargetDepartureTime.ToTimeSpan(), TargetArrivalTime.ToTimeSpan(),
                isRecurrent: false, null, null, TargetStartDate, string.Empty, string.Empty, TargetRunwayId, TargetGateId, _ => ValidFlightNumber));
    }

    [Test]
    public void CreateFlightWithScheduleAsync_AirportIdIsInvalid_ThrowsInvalidOperationException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            flightRouteService.CreateFlightWithScheduleAsync(TargetCompanyId, RouteTypeDeparture, InvalidAirportId, ValidCapacity,
                TargetDepartureTime.ToTimeSpan(), TargetArrivalTime.ToTimeSpan(),
                isRecurrent: false, null, null, TargetStartDate, string.Empty, string.Empty, TargetRunwayId, TargetGateId, _ => ValidFlightNumber));
    }

    [Test]
    public void CreateFlightWithScheduleAsync_RecurrentEndIsBeforeStart_ThrowsInvalidOperationException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            flightRouteService.CreateFlightWithScheduleAsync(TargetCompanyId, RouteTypeDeparture, TargetAirportId, ValidCapacity,
                TargetDepartureTime.ToTimeSpan(), TargetArrivalTime.ToTimeSpan(),
                isRecurrent: true, TargetStartDate, InvalidPastDate, null, DailyRecurrenceType, string.Empty, TargetRunwayId, TargetGateId, _ => ValidFlightNumber));
    }

    [Test]
    public void CreateFlightWithScheduleAsync_EqualDepartureAndArrivalTimes_ThrowsInvalidOperationException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            flightRouteService.CreateFlightWithScheduleAsync(TargetCompanyId, RouteTypeDeparture, TargetAirportId, ValidCapacity,
                TargetDepartureTime.ToTimeSpan(), TargetDepartureTime.ToTimeSpan(),
                isRecurrent: false, null, null, TargetStartDate, DailyRecurrenceType, string.Empty, TargetRunwayId, TargetGateId, _ => ValidFlightNumber));
    }

    [Test]
    public void CreateFlightWithScheduleAsync_InvalidRecurrenceType_ThrowsInvalidOperationException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            flightRouteService.CreateFlightWithScheduleAsync(TargetCompanyId, RouteTypeDeparture, TargetAirportId, ValidCapacity,
                TargetDepartureTime.ToTimeSpan(), TargetArrivalTime.ToTimeSpan(),
                isRecurrent: true, TargetStartDate, TargetStartDate, null, InvalidBiweeklyRecurrenceType, string.Empty, TargetRunwayId, TargetGateId, _ => ValidFlightNumber));
    }

    [Test]
    public void CreateFlightWithScheduleAsync_CustomIntervalIsInvalid_ThrowsInvalidOperationException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            flightRouteService.CreateFlightWithScheduleAsync(TargetCompanyId, RouteTypeDeparture, TargetAirportId, ValidCapacity,
                TargetDepartureTime.ToTimeSpan(), TargetArrivalTime.ToTimeSpan(),
                isRecurrent: true, TargetStartDate, TargetStartDate, null, CustomRecurrenceType, InvalidCustomDaysInterval, TargetRunwayId, TargetGateId, _ => ValidFlightNumber));
    }

    [Test]
    public void CreateFlightWithScheduleAsync_CustomIntervalIsZero_ThrowsInvalidOperationException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            flightRouteService.CreateFlightWithScheduleAsync(TargetCompanyId, RouteTypeDeparture, TargetAirportId, ValidCapacity,
                TargetDepartureTime.ToTimeSpan(), TargetArrivalTime.ToTimeSpan(),
                isRecurrent: true, TargetStartDate, TargetStartDate, null, CustomRecurrenceType, InvalidZeroDaysInterval, TargetRunwayId, TargetGateId, _ => ValidFlightNumber));
    }

    [Test]
    public async Task CreateFlightWithScheduleAsync_NonRecurrentFlight_Succeeds()
    {
        var (flightRepository, routeRepository, companyRepository, airportRepository) = ConfigureSuccessfulRepositories();
        var flightRouteService = CreateTestService(flightRepository, routeRepository, companyRepository, airportRepository);

        await flightRouteService.CreateFlightWithScheduleAsync(TargetCompanyId, RouteTypeDeparture, TargetAirportId, ValidCapacity,
            TargetDepartureTime.ToTimeSpan(), TargetArrivalTime.ToTimeSpan(),
            isRecurrent: false, null, null, TargetStartDate, string.Empty, string.Empty, TargetRunwayId, TargetGateId, _ => ValidFlightNumber);

        await routeRepository.Received(1).AddAsync(Arg.Any<Route>());
        await flightRepository.Received(1).AddAsync(Arg.Any<Flight>());
    }

    [Test]
    public async Task CreateFlightWithScheduleAsync_DailyRecurrence_Succeeds()
    {
        var (flightRepository, routeRepository, companyRepository, airportRepository) = ConfigureSuccessfulRepositories();
        var flightRouteService = CreateTestService(flightRepository, routeRepository, companyRepository, airportRepository);

        await flightRouteService.CreateFlightWithScheduleAsync(
            TargetCompanyId, RouteTypeDeparture, TargetAirportId, ValidCapacity,
            TargetDepartureTime.ToTimeSpan(), TargetArrivalTime.ToTimeSpan(),
            isRecurrent: true, TargetStartDate, TargetEndDate, null, DailyRecurrenceType, null!, TargetRunwayId, TargetGateId, _ => ValidFlightNumber);

        await routeRepository.Received(1).AddAsync(Arg.Any<Route>());
    }

    [Test]
    public async Task CreateFlightWithScheduleAsync_CustomRecurrence_Succeeds()
    {
        var (flightRepository, routeRepository, companyRepository, airportRepository) = ConfigureSuccessfulRepositories();
        var flightRouteService = CreateTestService(flightRepository, routeRepository, companyRepository, airportRepository);

        await flightRouteService.CreateFlightWithScheduleAsync(TargetCompanyId, RouteTypeDeparture, TargetAirportId, ValidCapacity,
            TargetDepartureTime.ToTimeSpan(), TargetArrivalTime.ToTimeSpan(),
            isRecurrent: true, TargetStartDate, TargetStartDate.AddDays(14), null, CustomRecurrenceType, ValidCustomDaysInterval, TargetRunwayId, TargetGateId, _ => ValidFlightNumber);

        await routeRepository.Received(1).AddAsync(Arg.Any<Route>());
    }

    [Test]
    public async Task SearchFlightsAsync_QueryIsEmpty_ReturnsAllFlights()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        var flights = new List<Flight>
        {
            new Flight { FlightNumber = ValidFlightNumber },
            new Flight { FlightNumber = ConflictingFlightNumber }
        };

        var result = (await flightRouteService.SearchFlightsAsync(flights, string.Empty)).ToList();

        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task SearchFlightsByNumberAsync_QueryMatchesFlightNumber_ReturnsFilteredFlights()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        var flights = new List<Flight>
        {
            new Flight { FlightNumber = ValidFlightNumber },
            new Flight { FlightNumber = ConflictingFlightNumber }
        };

        var result = (await flightRouteService.SearchFlightsByNumberAsync(flights, "fl")).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].FlightNumber, Is.EqualTo(ValidFlightNumber));
    }

    [Test]
    public async Task BuildFlightSummaryAsync_FlightIsValid_ReturnsCorrectSummary()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        var flight = new Flight
        {
            Id = TargetFlightId,
            FlightNumber = ValidFlightNumber,
            Date = TargetStartDate,
            Runway = new Runway { Id = TargetRunwayId, Name = TargetRunwayName },
            Gate = new Gate { Id = TargetGateId, GateName = TargetGateName },
            Route = new Route { Airport = new Airport { AirportCode = AirportCodeJfk, Name = AirportNameJfk } }
        };

        var summary = await flightRouteService.BuildFlightSummaryAsync(flight, ValidCrewText);

        Assert.That(summary.Id, Is.EqualTo(TargetFlightId));
        Assert.That(summary.FlightNumber, Is.EqualTo(ValidFlightNumber));
        Assert.That(summary.RunwayText, Is.EqualTo(TargetRunwayName));
        Assert.That(summary.GateText, Is.EqualTo(TargetGateName));
        Assert.That(summary.CrewText, Is.EqualTo(ValidCrewText));
        Assert.That(summary.DestinationText, Is.EqualTo(ExpectedDestinationTextJfk));
    }

    [Test]
    public async Task BuildFlightSummaryAsync_RunwayAndGateAreNull_ReturnsPlaceholders()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRouteService = CreateTestService(flightRepository, routeRepository);

        var flight = new Flight
        {
            Id = TargetFlightId,
            FlightNumber = ValidFlightNumber,
            Date = TargetStartDate,
            Runway = null!,
            Gate = null!,
            Route = null!
        };

        var summary = await flightRouteService.BuildFlightSummaryAsync(flight, ValidCrewText);

        Assert.That(summary.RunwayText, Is.EqualTo(MissingDestinationPlaceholder));
        Assert.That(summary.GateText, Is.EqualTo(MissingDestinationPlaceholder));
        Assert.That(summary.DestinationText, Is.EqualTo(MissingDestinationPlaceholder));
    }
}
