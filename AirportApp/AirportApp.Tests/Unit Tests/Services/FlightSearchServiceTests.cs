using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class FlightSearchServiceTests
{
    private const int TargetFlightId = 1;
    private const int TargetRouteCapacity = 100;
    private const int OccupiedSeatsCount = 40;
    private const int EnoughPassengers = 10;
    private const int TooManyPassengers = 80;
    private const string TargetLocation = "JFK";
    private const string WhitespaceLocation = "   ";
    private const string ValidPassengerInput = "5";
    private const string InvalidPassengerInput = "abc";
    private const string ZeroPassengerInput = "0";
    private const string NegativePassengerInput = "-3";
    private const string DepartureRouteType = "DEP";
    private const string ArrivalRouteType = "ARR";
    private static readonly DateTime TargetDate = new DateTime(2026, 6, 10);

    [Test]
    public async Task SearchFlightsAsync_LocationIsNull_ReturnsEmptyList()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = await flightSearchService.SearchFlightsAsync(null!, isDeparture: true, date: null, passengers: null);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task SearchFlightsAsync_LocationIsWhitespace_ReturnsEmptyList()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = await flightSearchService.SearchFlightsAsync(WhitespaceLocation, isDeparture: true, date: null, passengers: null);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task SearchFlightsAsync_LocationIsEmpty_ReturnsEmptyList()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = await flightSearchService.SearchFlightsAsync(string.Empty, isDeparture: true, date: null, passengers: null);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task SearchFlightsAsync_NoPassengerFilterApplied_ReturnsAllMatchingFlights()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var matchingFlights = new List<Flight>
        {
            new Flight { Id = TargetFlightId, Route = new Route { Capacity = TargetRouteCapacity } }
        };
        flightRepository.SearchFlightsAsync(TargetLocation, DepartureRouteType, null)
            .Returns(Task.FromResult<IEnumerable<Flight>>(matchingFlights));

        var flightSearchService = new FlightSearchService(flightRepository);
        var result = await flightSearchService.SearchFlightsAsync(TargetLocation, isDeparture: true, date: null, passengers: null);

        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task SearchFlightsAsync_EnoughSeatsAreAvailable_IncludesFlight()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var targetFlight = new Flight { Id = TargetFlightId, Route = new Route { Capacity = TargetRouteCapacity } };
        flightRepository.SearchFlightsAsync(TargetLocation, DepartureRouteType, null)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { targetFlight }));
        flightRepository.GetOccupiedSeatCountAsync(TargetFlightId)
            .Returns(Task.FromResult(OccupiedSeatsCount));

        var flightSearchService = new FlightSearchService(flightRepository);
        var result = await flightSearchService.SearchFlightsAsync(TargetLocation, isDeparture: true, date: null, passengers: EnoughPassengers);

        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task SearchFlightsAsync_NotEnoughSeatsAreAvailable_ExcludesFlight()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var targetFlight = new Flight { Id = TargetFlightId, Route = new Route { Capacity = TargetRouteCapacity } };
        flightRepository.SearchFlightsAsync(TargetLocation, DepartureRouteType, null)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { targetFlight }));
        flightRepository.GetOccupiedSeatCountAsync(TargetFlightId)
            .Returns(Task.FromResult(OccupiedSeatsCount));

        var flightSearchService = new FlightSearchService(flightRepository);
        var result = await flightSearchService.SearchFlightsAsync(TargetLocation, isDeparture: true, date: null, passengers: TooManyPassengers);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task SearchFlightsAsync_IsDepartureIsFalse_UsesArrivalRouteType()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.SearchFlightsAsync(TargetLocation, ArrivalRouteType, null)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var flightSearchService = new FlightSearchService(flightRepository);
        await flightSearchService.SearchFlightsAsync(TargetLocation, isDeparture: false, date: null, passengers: null);

        await flightRepository.Received(1).SearchFlightsAsync(TargetLocation, ArrivalRouteType, null);
    }

    [Test]
    public async Task SearchFlightsAsync_IsDepartureIsTrue_UsesDepartureRouteType()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.SearchFlightsAsync(TargetLocation, DepartureRouteType, null)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var flightSearchService = new FlightSearchService(flightRepository);
        await flightSearchService.SearchFlightsAsync(TargetLocation, isDeparture: true, date: null, passengers: null);

        await flightRepository.Received(1).SearchFlightsAsync(TargetLocation, DepartureRouteType, null);
    }

    [Test]
    public async Task SearchFlightsAsync_DateIsProvided_PassesDateToRepository()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.SearchFlightsAsync(TargetLocation, DepartureRouteType, TargetDate)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var flightSearchService = new FlightSearchService(flightRepository);
        await flightSearchService.SearchFlightsAsync(TargetLocation, isDeparture: true, date: TargetDate, passengers: null);

        await flightRepository.Received(1).SearchFlightsAsync(TargetLocation, DepartureRouteType, TargetDate);
    }

    [Test]
    public void ParsePassengerCount_InputIsNull_ReturnsNull()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = flightSearchService.ParsePassengerCount(null!);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParsePassengerCount_InputIsEmpty_ReturnsNull()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = flightSearchService.ParsePassengerCount(string.Empty);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParsePassengerCount_ValidPositiveNumber_ReturnsParsedValue()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = flightSearchService.ParsePassengerCount(ValidPassengerInput);

        Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public void ParsePassengerCount_InvalidString_ReturnsOne()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = flightSearchService.ParsePassengerCount(InvalidPassengerInput);

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void ParsePassengerCount_InputIsZero_ReturnsOne()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = flightSearchService.ParsePassengerCount(ZeroPassengerInput);

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void ParsePassengerCount_InputIsNegative_ReturnsOne()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = flightSearchService.ParsePassengerCount(NegativePassengerInput);

        Assert.That(result, Is.EqualTo(1));
    }
}
