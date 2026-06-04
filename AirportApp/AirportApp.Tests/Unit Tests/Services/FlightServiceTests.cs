using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class FlightServiceTests
{
    private const string FirstFlightNumber = "AA100";
    private const string SecondFlightNumber = "BB200";
    private const int NumberOfFlights = 2;
    private const int NegativeFlightId = -1;
    private const int ZeroFlightId = 0;
    private const int ValidFlightId = 1;
    private const int InvalidFlightId = 99;
    private const int InvalidRouteId = 0;
    private const int ValidRouteId = 5;
    private const int ValidRunwayId = 3;
    private const int ValidGateId = 1;
    private const int NewRunwayId = 7;
    private const int NewGateId = 9;
    private static readonly DateTime FlightDate = new DateTime(2024, 5, 1);
    private static readonly DateTime NewFlightDate = new DateTime(2025, 6, 15);
    private static readonly DateTime NewFlightDate2 = new DateTime(2025, 12, 1);

    [Test]
    public async Task GetFlightByIdAsync_NegativeId_ReturnsNull()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        var result = await flightService.GetFlightByIdAsync(NegativeFlightId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetFlightByIdAsync_ZeroId_ReturnsNull()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        var result = await flightService.GetFlightByIdAsync(ZeroFlightId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetFlightByIdAsync_FlightFound_ReturnsFlight()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var targetFlight = new Flight { FlightNumber = FirstFlightNumber };
        flightRepository.GetByIdAsync(ValidFlightId).Returns(Task.FromResult<Flight?>(targetFlight));

        var flightService = new FlightService(flightRepository);
        var result = await flightService.GetFlightByIdAsync(ValidFlightId);

        Assert.That(result, Is.EqualTo(targetFlight));
    }

    [Test]
    public async Task GetFlightsByRouteIdAsync_InvalidRouteId_ReturnsEmptyList()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        var result = await flightService.GetFlightsByRouteIdAsync(InvalidRouteId);

        Assert.That(result, Is.Empty);
        await flightRepository.DidNotReceive().GetByRouteIdAsync(Arg.Any<int>());
    }

    [Test]
    public async Task GetFlightsByRouteIdAsync_FlightsFound_ReturnsFlights()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flights = new List<Flight> { new Flight { FlightNumber = FirstFlightNumber } };
        flightRepository.GetByRouteIdAsync(ValidRouteId).Returns(Task.FromResult<IEnumerable<Flight>>(flights));

        var flightService = new FlightService(flightRepository);
        var result = await flightService.GetFlightsByRouteIdAsync(ValidRouteId);

        Assert.That(result, Is.EqualTo(flights));
    }

    [Test]
    public void AddFlightAsync_NullFlightNumber_ThrowsArgumentException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            flightService.AddFlightAsync(null!, ValidRouteId, DateTime.Now, ValidRunwayId, ValidGateId));
    }

    [Test]
    public void AddFlightAsync_EmptyFlightNumber_ThrowsArgumentException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            flightService.AddFlightAsync(string.Empty, ValidRouteId, DateTime.Now, ValidRunwayId, ValidGateId));
    }

    [Test]
    public void AddFlightAsync_WhitespaceFlightNumber_ThrowsArgumentException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            flightService.AddFlightAsync(" ", ValidRouteId, DateTime.Now, ValidRunwayId, ValidGateId));
    }

    [Test]
    public void AddFlightAsync_InvalidRouteId_ThrowsArgumentException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            flightService.AddFlightAsync(FirstFlightNumber, InvalidRouteId, DateTime.Now, ValidRunwayId, ValidGateId));
    }

    [Test]
    public async Task AddFlightAsync_ValidData_ReturnsNewFlightId()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.AddAsync(Arg.Any<Flight>()).Returns(Task.FromResult(ValidFlightId));

        var flightService = new FlightService(flightRepository);
        var result = await flightService.AddFlightAsync(FirstFlightNumber, ValidRouteId, DateTime.Now, ValidRunwayId, ValidGateId);

        Assert.That(result, Is.EqualTo(ValidFlightId));
        await flightRepository.Received(1).AddAsync(Arg.Any<Flight>());
    }

    [Test]
    public void UpdateFlightAsync_FlightNotFound_ThrowsInvalidOperationException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.GetByIdAsync(InvalidFlightId).Returns(Task.FromResult<Flight?>(null));

        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() => flightService.UpdateFlightAsync(InvalidFlightId));
    }

    [Test]
    public async Task UpdateFlightAsync_FlightNumberNotProvided_UpdatesOnlyDate()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flight = new Flight { FlightNumber = FirstFlightNumber, Date = FlightDate };
        flightRepository.GetByIdAsync(ValidFlightId).Returns(Task.FromResult<Flight?>(flight));

        var flightService = new FlightService(flightRepository);
        await flightService.UpdateFlightAsync(ValidFlightId, date: NewFlightDate);

        Assert.That(flight.Date, Is.EqualTo(NewFlightDate));
        Assert.That(flight.FlightNumber, Is.EqualTo(FirstFlightNumber));
        await flightRepository.Received(1).UpdateAsync(flight);
    }

    [Test]
    public async Task UpdateFlightAsync_DateNotProvided_UpdatesOnlyFlightNumber()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flight = new Flight { FlightNumber = FirstFlightNumber, Date = FlightDate };
        flightRepository.GetByIdAsync(ValidFlightId).Returns(Task.FromResult<Flight?>(flight));

        var flightService = new FlightService(flightRepository);
        await flightService.UpdateFlightAsync(ValidFlightId, flightNumber: SecondFlightNumber);

        Assert.That(flight.FlightNumber, Is.EqualTo(SecondFlightNumber));
        Assert.That(flight.Date, Is.EqualTo(FlightDate));
        await flightRepository.Received(1).UpdateAsync(flight);
    }

    [Test]
    public async Task UpdateFlightAsync_OtherFieldsNotProvided_UpdatesOnlyRunwayId()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flight = new Flight { FlightNumber = FirstFlightNumber, Runway = new Runway { Id = ValidRunwayId } };
        flightRepository.GetByIdAsync(ValidFlightId).Returns(Task.FromResult<Flight?>(flight));

        var flightService = new FlightService(flightRepository);
        await flightService.UpdateFlightAsync(ValidFlightId, runwayId: NewRunwayId);

        Assert.That(flight.Runway.Id, Is.EqualTo(NewRunwayId));
        Assert.That(flight.FlightNumber, Is.EqualTo(FirstFlightNumber));
        await flightRepository.Received(1).UpdateAsync(flight);
    }

    [Test]
    public async Task UpdateFlightAsync_OtherFieldsNotProvided_UpdatesOnlyGateId()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flight = new Flight { FlightNumber = FirstFlightNumber, Gate = new Gate { Id = ValidGateId } };
        flightRepository.GetByIdAsync(ValidFlightId).Returns(Task.FromResult<Flight?>(flight));

        var flightService = new FlightService(flightRepository);
        await flightService.UpdateFlightAsync(ValidFlightId, gateId: NewGateId);

        Assert.That(flight.Gate.Id, Is.EqualTo(NewGateId));
        Assert.That(flight.FlightNumber, Is.EqualTo(FirstFlightNumber));
        await flightRepository.Received(1).UpdateAsync(flight);
    }

    [Test]
    public async Task UpdateFlightAsync_AllFieldsProvided_UpdatesAllFields()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flight = new Flight
        {
            FlightNumber = FirstFlightNumber,
            Date = FlightDate,
            Runway = new Runway { Id = ValidRunwayId },
            Gate = new Gate { Id = ValidGateId }
        };
        flightRepository.GetByIdAsync(ValidFlightId).Returns(Task.FromResult<Flight?>(flight));

        var flightService = new FlightService(flightRepository);
        await flightService.UpdateFlightAsync(ValidFlightId, NewFlightDate2, SecondFlightNumber, NewRunwayId, NewGateId);

        Assert.That(flight.Date, Is.EqualTo(NewFlightDate2));
        Assert.That(flight.FlightNumber, Is.EqualTo(SecondFlightNumber));
        Assert.That(flight.Runway.Id, Is.EqualTo(NewRunwayId));
        Assert.That(flight.Gate.Id, Is.EqualTo(NewGateId));
        await flightRepository.Received(1).UpdateAsync(flight);
    }

    [Test]
    public void DeleteFlightAsync_ZeroId_ThrowsArgumentException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() => flightService.DeleteFlightAsync(ZeroFlightId));
    }

    [Test]
    public void DeleteFlightAsync_NegativeId_ThrowsArgumentException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() => flightService.DeleteFlightAsync(NegativeFlightId));
    }

    [Test]
    public void DeleteFlightAsync_FlightNotFound_ThrowsInvalidOperationException()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.GetByIdAsync(ValidFlightId).Returns(Task.FromResult<Flight?>(null));

        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() => flightService.DeleteFlightAsync(ValidFlightId));
    }

    [Test]
    public async Task DeleteFlightAsync_IdIsValid_CallsRepositoryDelete()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.GetByIdAsync(ValidFlightId).Returns(Task.FromResult<Flight?>(new Flight()));
        flightRepository.DeleteAsync(ValidFlightId).Returns(Task.CompletedTask);

        var flightService = new FlightService(flightRepository);
        await flightService.DeleteFlightAsync(ValidFlightId);

        await flightRepository.Received(1).DeleteAsync(ValidFlightId);
    }
}
