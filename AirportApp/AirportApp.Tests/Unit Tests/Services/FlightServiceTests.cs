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
    public async Task GetAllFlightsAsync_ReturnsAllFlights_Always()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flights = new List<Flight>
        {
            new Flight { FlightNumber = FirstFlightNumber },
            new Flight { FlightNumber = SecondFlightNumber }
        };
        flightRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Flight>>(flights));

        var flightService = new FlightService(flightRepository);
        var result = (await flightService.GetAllFlightsAsync()).ToList();

        Assert.That(result.Count, Is.EqualTo(NumberOfFlights));
        Assert.That(result, Is.EqualTo(flights));
    }

    [Test]
    public async Task GetFlightByIdAsync_ReturnsNull_ForNegativeId()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        var result = await flightService.GetFlightByIdAsync(NegativeFlightId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetFlightByIdAsync_ReturnsNull_ForZeroId()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        var result = await flightService.GetFlightByIdAsync(ZeroFlightId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetFlightByIdAsync_ReturnsFlight_WhenFound()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var targetFlight = new Flight { FlightNumber = FirstFlightNumber };
        flightRepository.GetByIdAsync(ValidFlightId).Returns(Task.FromResult<Flight?>(targetFlight));

        var flightService = new FlightService(flightRepository);
        var result = await flightService.GetFlightByIdAsync(ValidFlightId);

        Assert.That(result, Is.EqualTo(targetFlight));
    }

    [Test]
    public async Task GetFlightsByRouteIdAsync_ReturnsEmptyList_ForInvalidRouteId()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        var result = await flightService.GetFlightsByRouteIdAsync(InvalidRouteId);

        Assert.That(result, Is.Empty);
        await flightRepository.DidNotReceive().GetByRouteIdAsync(Arg.Any<int>());
    }

    [Test]
    public async Task GetFlightsByRouteIdAsync_ReturnsFlights_WhenFound()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flights = new List<Flight> { new Flight { FlightNumber = FirstFlightNumber } };
        flightRepository.GetByRouteIdAsync(ValidRouteId).Returns(Task.FromResult<IEnumerable<Flight>>(flights));

        var flightService = new FlightService(flightRepository);
        var result = await flightService.GetFlightsByRouteIdAsync(ValidRouteId);

        Assert.That(result, Is.EqualTo(flights));
    }

    [Test]
    public void AddFlightAsync_ThrowsArgumentException_ForNullFlightNumber()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            flightService.AddFlightAsync(null!, ValidRouteId, DateTime.Now, ValidRunwayId, ValidGateId));
    }

    [Test]
    public void AddFlightAsync_ThrowsArgumentException_ForEmptyFlightNumber()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            flightService.AddFlightAsync(string.Empty, ValidRouteId, DateTime.Now, ValidRunwayId, ValidGateId));
    }

    [Test]
    public void AddFlightAsync_ThrowsArgumentException_ForWhitespaceFlightNumber()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            flightService.AddFlightAsync(" ", ValidRouteId, DateTime.Now, ValidRunwayId, ValidGateId));
    }

    [Test]
    public void AddFlightAsync_ThrowsArgumentException_ForInvalidRouteId()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            flightService.AddFlightAsync(FirstFlightNumber, InvalidRouteId, DateTime.Now, ValidRunwayId, ValidGateId));
    }

    [Test]
    public async Task AddFlightAsync_ReturnsNewFlightId_ForValidData()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.AddAsync(Arg.Any<Flight>()).Returns(Task.FromResult(ValidFlightId));

        var flightService = new FlightService(flightRepository);
        var result = await flightService.AddFlightAsync(FirstFlightNumber, ValidRouteId, DateTime.Now, ValidRunwayId, ValidGateId);

        Assert.That(result, Is.EqualTo(ValidFlightId));
        await flightRepository.Received(1).AddAsync(Arg.Any<Flight>());
    }

    [Test]
    public void UpdateFlightAsync_ThrowsInvalidOperationException_WhenFlightNotFound()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.GetByIdAsync(InvalidFlightId).Returns(Task.FromResult<Flight?>(null));

        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() => flightService.UpdateFlightAsync(InvalidFlightId));
    }

    [Test]
    public async Task UpdateFlightAsync_UpdatesOnlyDate_WhenFlightNumberIsNotProvided()
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
    public async Task UpdateFlightAsync_UpdatesOnlyFlightNumber_WhenDateIsNotProvided()
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
    public async Task UpdateFlightAsync_UpdatesOnlyRunwayId_WhenOtherFieldsAreNotProvided()
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
    public async Task UpdateFlightAsync_UpdatesOnlyGateId_WhenOtherFieldsAreNotProvided()
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
    public async Task UpdateFlightAsync_UpdatesAllFields_WhenAllFieldsAreProvided()
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
    public void DeleteFlightAsync_ThrowsArgumentException_ForZeroId()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() => flightService.DeleteFlightAsync(ZeroFlightId));
    }

    [Test]
    public void DeleteFlightAsync_ThrowsArgumentException_ForNegativeId()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() => flightService.DeleteFlightAsync(NegativeFlightId));
    }

    [Test]
    public void DeleteFlightAsync_ThrowsInvalidOperationException_WhenFlightNotFound()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.GetByIdAsync(ValidFlightId).Returns(Task.FromResult<Flight?>(null));

        var flightService = new FlightService(flightRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() => flightService.DeleteFlightAsync(ValidFlightId));
    }

    [Test]
    public async Task DeleteFlightAsync_CallsRepositoryDelete_WhenIdIsValid()
    {
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.GetByIdAsync(ValidFlightId).Returns(Task.FromResult<Flight?>(new Flight()));
        flightRepository.DeleteAsync(ValidFlightId).Returns(Task.CompletedTask);

        var flightService = new FlightService(flightRepository);
        await flightService.DeleteFlightAsync(ValidFlightId);

        await flightRepository.Received(1).DeleteAsync(ValidFlightId);
    }
}
