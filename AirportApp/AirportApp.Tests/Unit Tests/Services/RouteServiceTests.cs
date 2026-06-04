using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class RouteServiceTests
{
    private const int ValidRouteId = 1;
    private const int InvalidRouteId = 99;
    private const string DepartureRouteType = "DEP";
    private const string ArrivalRouteType = "ARR";
    private const string DepartureRouteTypeLowercase = "dep";
    private const string ArrivalRouteTypeLowercase = "arr";
    private const string DepartureRouteTypeFull = "DEPARTURE";
    private const string ArrivalRouteTypeFull = "ARRIVAL";
    private const string DepartureRouteTypeLowercaseFull = "departure";
    private const string ArrivalRouteTypeLowercaseFull = "arrival";
    private const int NumberOfRoutes = 2;
    private const string DashString = "-";
    private const string UnknownRouteType = "transit";
    private const string ArrivalTimeString = "14:30";
    private const string DepartureTimeString = "10:00";
    private const int DefaultCompanyId = 1;
    private const int DefaultAirportId = 1;
    private const int DefaultRecurrenceInterval = 0;
    private const int DefaultCapacity = 100;
    private const string DefaultFlightNumber = "FL001";
    private const int DefaultGateId = 1;
    private const int DefaultRunwayId = 1;
    private const string DifferentFlightName = "FL999";
    private const string Wrap2FlightName = "WRAP2";

    private static readonly TimeOnly ArrivalTime = new TimeOnly(14, 30);
    private static readonly TimeOnly DepartureTime = new TimeOnly(10, 0);
    private static readonly TimeOnly DefaultDepartureTime = new TimeOnly(10, 0);
    private static readonly TimeOnly DefaultArrivalTime = new TimeOnly(12, 0);
    private static readonly TimeOnly DefaultWrapDepartureTime = new TimeOnly(23, 0);
    private static readonly TimeOnly DefaultWrapArrivalTime = new TimeOnly(1, 0);
    private static readonly TimeOnly DefaultWrap2DepartureTime = new TimeOnly(23, 30);
    private static readonly TimeOnly DefaultWrap2ArrivalTime = new TimeOnly(0, 30);
    private static readonly DateTime DefaultDate = new DateTime(2025, 1, 1);
    private static readonly DateTime DifferentDate = new DateTime(2025, 2, 1);

    private static RouteService BuildService(
        IRouteRepository routeRepository,
        IFlightRepository flightRepository,
        ICompanyRepository? companyRepository = null,
        IAirportRepository? airportRepository = null)
    {
        return new RouteService(
            routeRepository,
            flightRepository,
            companyRepository ?? Substitute.For<ICompanyRepository>(),
            airportRepository ?? Substitute.For<IAirportRepository>());
    }

    [Test]
    public async Task GetRouteByIdAsync_ReturnsRoute_WhenRouteExists()
    {
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var targetRoute = new Route { RouteType = DepartureRouteType };
        routeRepository.GetByIdAsync(ValidRouteId).Returns(Task.FromResult<Route?>(targetRoute));

        var routeService = BuildService(routeRepository, flightRepository);
        var result = await routeService.GetRouteByIdAsync(ValidRouteId);

        Assert.That(result, Is.EqualTo(targetRoute));
    }

    [Test]
    public async Task GetRouteByIdAsync_ReturnsNull_WhenRouteNotFound()
    {
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        routeRepository.GetByIdAsync(InvalidRouteId).Returns(Task.FromResult<Route?>(null));

        var routeService = BuildService(routeRepository, flightRepository);
        var result = await routeService.GetRouteByIdAsync(InvalidRouteId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllRoutesAsync_ReturnsAllRoutes_Always()
    {
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var routes = new List<Route> { new Route(), new Route() };
        routeRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Route>>(routes));

        var routeService = BuildService(routeRepository, flightRepository);
        var result = (await routeService.GetAllRoutesAsync()).ToList();

        Assert.That(result.Count, Is.EqualTo(NumberOfRoutes));
    }

    [Test]
    public void NormalizeFlightType_ReturnsDash_ForNull()
    {
        var routeService = BuildService(Substitute.For<IRouteRepository>(), Substitute.For<IFlightRepository>());

        Assert.That(routeService.NormalizeFlightType(null), Is.EqualTo(DashString));
    }

    [Test]
    public void NormalizeFlightType_ReturnsDash_ForEmptyString()
    {
        var routeService = BuildService(Substitute.For<IRouteRepository>(), Substitute.For<IFlightRepository>());

        Assert.That(routeService.NormalizeFlightType(string.Empty), Is.EqualTo(DashString));
    }

    [Test]
    public void NormalizeFlightType_ReturnsDash_ForWhitespace()
    {
        var routeService = BuildService(Substitute.For<IRouteRepository>(), Substitute.For<IFlightRepository>());

        Assert.That(routeService.NormalizeFlightType("  "), Is.EqualTo(DashString));
    }

    [Test]
    public void NormalizeFlightType_ReturnsARR_ForArrivalVariants()
    {
        var routeService = BuildService(Substitute.For<IRouteRepository>(), Substitute.For<IFlightRepository>());

        Assert.That(routeService.NormalizeFlightType(ArrivalRouteTypeLowercase), Is.EqualTo(ArrivalRouteType));
        Assert.That(routeService.NormalizeFlightType(ArrivalRouteType), Is.EqualTo(ArrivalRouteType));
        Assert.That(routeService.NormalizeFlightType(ArrivalRouteTypeLowercaseFull), Is.EqualTo(ArrivalRouteType));
        Assert.That(routeService.NormalizeFlightType(ArrivalRouteTypeFull), Is.EqualTo(ArrivalRouteType));
    }

    [Test]
    public void NormalizeFlightType_ReturnsDEP_ForDepartureVariants()
    {
        var routeService = BuildService(Substitute.For<IRouteRepository>(), Substitute.For<IFlightRepository>());

        Assert.That(routeService.NormalizeFlightType(DepartureRouteTypeLowercase), Is.EqualTo(DepartureRouteType));
        Assert.That(routeService.NormalizeFlightType(DepartureRouteType), Is.EqualTo(DepartureRouteType));
        Assert.That(routeService.NormalizeFlightType(DepartureRouteTypeLowercaseFull), Is.EqualTo(DepartureRouteType));
        Assert.That(routeService.NormalizeFlightType(DepartureRouteTypeFull), Is.EqualTo(DepartureRouteType));
    }

    [Test]
    public void NormalizeFlightType_ReturnsUppercasedValue_ForUnknownType()
    {
        var routeService = BuildService(Substitute.For<IRouteRepository>(), Substitute.For<IFlightRepository>());

        Assert.That(routeService.NormalizeFlightType(UnknownRouteType), Is.EqualTo(UnknownRouteType.ToUpper()));
    }

    [Test]
    public void GetRelevantTime_ReturnsDash_ForNullRoute()
    {
        var routeService = BuildService(Substitute.For<IRouteRepository>(), Substitute.For<IFlightRepository>());

        Assert.That(routeService.GetRelevantTime(null), Is.EqualTo(DashString));
    }

    [Test]
    public void GetRelevantTime_ReturnsArrivalTime_ForARRRoute()
    {
        var routeService = BuildService(Substitute.For<IRouteRepository>(), Substitute.For<IFlightRepository>());
        var arrivalRoute = new Route { RouteType = ArrivalRouteType, ArrivalTime = ArrivalTime, DepartureTime = DepartureTime };

        Assert.That(routeService.GetRelevantTime(arrivalRoute), Is.EqualTo(ArrivalTimeString));
    }

    [Test]
    public void GetRelevantTime_ReturnsDepartureTime_ForDEPRoute()
    {
        var routeService = BuildService(Substitute.For<IRouteRepository>(), Substitute.For<IFlightRepository>());
        var departureRoute = new Route { RouteType = DepartureRouteType, ArrivalTime = ArrivalTime, DepartureTime = DepartureTime };

        Assert.That(routeService.GetRelevantTime(departureRoute), Is.EqualTo(DepartureTimeString));
    }

    [Test]
    public async Task AddWithInitialFlightAsync_Succeeds_WhenNoConflicts()
    {
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var companyRepository = Substitute.For<ICompanyRepository>();
        var airportRepository = Substitute.For<IAirportRepository>();

        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));
        companyRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Company?>(new Company()));
        airportRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Airport?>(new Airport()));
        routeRepository.AddAsync(Arg.Any<Route>()).Returns(Task.FromResult(ValidRouteId));

        var routeService = BuildService(routeRepository, flightRepository, companyRepository, airportRepository);

        int result = await routeService.AddWithInitialFlightAsync(DefaultCompanyId, DefaultAirportId, DepartureRouteType, DefaultRecurrenceInterval,
            DefaultDate, DefaultDate, DepartureTime, ArrivalTime, DefaultCapacity, DefaultFlightNumber, DefaultRunwayId, DefaultGateId);

        Assert.That(result, Is.EqualTo(ValidRouteId));
        await routeRepository.Received(1).AddAsync(Arg.Any<Route>());
        await flightRepository.Received(1).AddAsync(Arg.Any<Flight>());
    }

    [Test]
    public void AddWithInitialFlightAsync_ThrowsInvalidOperationException_WhenTimesOverlap()
    {
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();

        var existingFlight = new Flight
        {
            Date = DefaultDate,
            Gate = new Gate { Id = DefaultGateId },
            Runway = new Runway { Id = DefaultRunwayId },
            Route = new Route { Id = ValidRouteId }
        };
        var conflictingRoute = new Route
        {
            DepartureTime = DefaultWrap2DepartureTime,
            ArrivalTime = DefaultWrap2ArrivalTime
        };

        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { existingFlight }));
        routeRepository.GetByIdAsync(ValidRouteId).Returns(Task.FromResult<Route?>(conflictingRoute));

        var routeService = BuildService(routeRepository, flightRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            routeService.AddWithInitialFlightAsync(DefaultCompanyId, DefaultAirportId, DepartureRouteType, DefaultRecurrenceInterval,
                DefaultDate, DefaultDate, DefaultWrapDepartureTime, DefaultWrapArrivalTime, DefaultCapacity, Wrap2FlightName, DefaultRunwayId, DefaultGateId));
    }

    [Test]
    public async Task AddWithInitialFlightAsync_ContinuesProcessing_WhenExistingRouteIsNull()
    {
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var companyRepository = Substitute.For<ICompanyRepository>();
        var airportRepository = Substitute.For<IAirportRepository>();

        var existingFlight = new Flight
        {
            Date = DefaultDate,
            Gate = new Gate { Id = DefaultGateId },
            Runway = new Runway { Id = DefaultRunwayId },
            Route = new Route { Id = ValidRouteId }
        };

        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { existingFlight }));
        routeRepository.GetByIdAsync(ValidRouteId).Returns(Task.FromResult<Route?>(null));
        companyRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Company?>(new Company()));
        airportRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Airport?>(new Airport()));
        routeRepository.AddAsync(Arg.Any<Route>()).Returns(Task.FromResult(ValidRouteId));

        var routeService = BuildService(routeRepository, flightRepository, companyRepository, airportRepository);

        int result = await routeService.AddWithInitialFlightAsync(DefaultCompanyId, DefaultAirportId, DepartureRouteType, DefaultRecurrenceInterval,
            DefaultDate, DefaultDate, DefaultDepartureTime, DefaultArrivalTime, DefaultCapacity, DefaultFlightNumber, DefaultGateId, DefaultRunwayId);

        Assert.That(result, Is.EqualTo(ValidRouteId));
    }

    [Test]
    public async Task AddWithInitialFlightAsync_SkipsFlights_OnDifferentDate()
    {
        var routeRepository = Substitute.For<IRouteRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var companyRepository = Substitute.For<ICompanyRepository>();
        var airportRepository = Substitute.For<IAirportRepository>();

        var otherDayFlight = new Flight
        {
            Date = DifferentDate,
            Gate = new Gate { Id = DefaultGateId },
            Runway = new Runway { Id = DefaultRunwayId },
            Route = new Route { Id = ValidRouteId }
        };

        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { otherDayFlight }));
        companyRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Company?>(new Company()));
        airportRepository.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Airport?>(new Airport()));
        routeRepository.AddAsync(Arg.Any<Route>()).Returns(Task.FromResult(ValidRouteId));

        var routeService = BuildService(routeRepository, flightRepository, companyRepository, airportRepository);

        int result = await routeService.AddWithInitialFlightAsync(DefaultCompanyId, DefaultAirportId, DepartureRouteType, DefaultRecurrenceInterval,
            DefaultDate, DefaultDate, DefaultDepartureTime, DefaultArrivalTime, DefaultCapacity, DifferentFlightName, DefaultGateId, DefaultRunwayId);

        Assert.That(result, Is.EqualTo(ValidRouteId));
        await routeRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>());
    }
}
