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
    public async Task SearchFlightsAsync_ReturnsEmptyList_WhenLocationIsNull()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = await flightSearchService.SearchFlightsAsync(null!, isDeparture: true, date: null, passengers: null);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task SearchFlightsAsync_ReturnsEmptyList_WhenLocationIsWhitespace()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = await flightSearchService.SearchFlightsAsync(WhitespaceLocation, isDeparture: true, date: null, passengers: null);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task SearchFlightsAsync_ReturnsEmptyList_WhenLocationIsEmpty()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = await flightSearchService.SearchFlightsAsync(string.Empty, isDeparture: true, date: null, passengers: null);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task SearchFlightsAsync_ReturnsAllMatchingFlights_WhenNoPassengerFilterIsApplied()
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
    public async Task SearchFlightsAsync_IncludesFlight_WhenEnoughSeatsAreAvailable()
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
    public async Task SearchFlightsAsync_ExcludesFlight_WhenNotEnoughSeatsAreAvailable()
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
    public async Task SearchFlightsAsync_UsesArrivalRouteType_WhenIsDepartureIsFalse()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.SearchFlightsAsync(TargetLocation, ArrivalRouteType, null)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var flightSearchService = new FlightSearchService(flightRepository);
        await flightSearchService.SearchFlightsAsync(TargetLocation, isDeparture: false, date: null, passengers: null);

        await flightRepository.Received(1).SearchFlightsAsync(TargetLocation, ArrivalRouteType, null);
    }

    [Test]
    public async Task SearchFlightsAsync_UsesDepartureRouteType_WhenIsDepartureIsTrue()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.SearchFlightsAsync(TargetLocation, DepartureRouteType, null)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var flightSearchService = new FlightSearchService(flightRepository);
        await flightSearchService.SearchFlightsAsync(TargetLocation, isDeparture: true, date: null, passengers: null);

        await flightRepository.Received(1).SearchFlightsAsync(TargetLocation, DepartureRouteType, null);
    }

    [Test]
    public async Task SearchFlightsAsync_PassesDateToRepository_WhenDateIsProvided()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.SearchFlightsAsync(TargetLocation, DepartureRouteType, TargetDate)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var flightSearchService = new FlightSearchService(flightRepository);
        await flightSearchService.SearchFlightsAsync(TargetLocation, isDeparture: true, date: TargetDate, passengers: null);

        await flightRepository.Received(1).SearchFlightsAsync(TargetLocation, DepartureRouteType, TargetDate);
    }

    [Test]
    public void ParsePassengerCount_ReturnsNull_WhenInputIsNull()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = flightSearchService.ParsePassengerCount(null!);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParsePassengerCount_ReturnsNull_WhenInputIsEmpty()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = flightSearchService.ParsePassengerCount(string.Empty);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParsePassengerCount_ReturnsParsedValue_WhenInputIsValidPositiveNumber()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = flightSearchService.ParsePassengerCount(ValidPassengerInput);

        Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public void ParsePassengerCount_ReturnsOne_WhenInputIsInvalidString()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = flightSearchService.ParsePassengerCount(InvalidPassengerInput);

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void ParsePassengerCount_ReturnsOne_WhenInputIsZero()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = flightSearchService.ParsePassengerCount(ZeroPassengerInput);

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void ParsePassengerCount_ReturnsOne_WhenInputIsNegative()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightSearchService = new FlightSearchService(flightRepository);

        var result = flightSearchService.ParsePassengerCount(NegativePassengerInput);

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task GetFlightByIdAsync_ReturnsFlight_WhenFound()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var targetFlight = new Flight { Id = TargetFlightId };
        flightRepository.GetByIdAsync(TargetFlightId).Returns(Task.FromResult<Flight?>(targetFlight));

        var flightSearchService = new FlightSearchService(flightRepository);
        var result = await flightSearchService.GetFlightByIdAsync(TargetFlightId);

        Assert.That(result, Is.EqualTo(targetFlight));
    }

    [Test]
    public async Task GetFlightsByRouteAsync_WithGivenRouteParameters_DelegatesSearchToRepository()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var expectedFlights = new List<Flight> { new Flight { Id = TargetFlightId } };
        flightRepository.SearchFlightsAsync(TargetLocation, DepartureRouteType, TargetDate)
            .Returns(Task.FromResult<IEnumerable<Flight>>(expectedFlights));

        var flightSearchService = new FlightSearchService(flightRepository);
        var result = await flightSearchService.GetFlightsByRouteAsync(TargetLocation, DepartureRouteType, TargetDate);

        Assert.That(result, Is.EqualTo(expectedFlights));
    }

    [Test]
    public async Task GetOccupiedSeatCountAsync_ReturnsCount_ForValidFlightId()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.GetOccupiedSeatCountAsync(TargetFlightId).Returns(Task.FromResult(OccupiedSeatsCount));

        var flightSearchService = new FlightSearchService(flightRepository);
        var result = await flightSearchService.GetOccupiedSeatCountAsync(TargetFlightId);

        Assert.That(result, Is.EqualTo(OccupiedSeatsCount));
    }
}
