using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class GateServiceTests
{
    private const string FirstGateName = "A1";
    private const string SecondGateName = "B2";
    private const int ValidGateId = 1;
    private const int NegativeGateId = -1;
    private const int InexistentGateId = 0;
    private const int NumberOfGates = 2;

    [Test]
    public async Task GetAllGatesAsync_Called_ReturnsAllGates()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var gates = new List<Gate>
        {
            new Gate { GateName = FirstGateName },
            new Gate { GateName = SecondGateName }
        };
        gateRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Gate>>(gates));

        var gateService = new GateService(gateRepository, flightRepository);
        var result = (await gateService.GetAllGatesAsync()).ToList();

        Assert.That(result.Count, Is.EqualTo(NumberOfGates));
        Assert.That(result, Is.EqualTo(gates));
    }

    [Test]
    public async Task GetGateByIdAsync_ZeroId_ReturnsNull()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var gateService = new GateService(gateRepository, flightRepository);

        var result = await gateService.GetGateByIdAsync(InexistentGateId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetGateByIdAsync_NegativeId_ReturnsNull()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var gateService = new GateService(gateRepository, flightRepository);

        var result = await gateService.GetGateByIdAsync(NegativeGateId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetGateByIdAsync_GateFound_ReturnsGate()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var targetGate = new Gate { GateName = FirstGateName };
        gateRepository.GetByIdAsync(ValidGateId).Returns(Task.FromResult<Gate?>(targetGate));

        var gateService = new GateService(gateRepository, flightRepository);
        var result = await gateService.GetGateByIdAsync(ValidGateId);

        Assert.That(result, Is.EqualTo(targetGate));
    }

    [Test]
    public void AddGateAsync_NullName_ThrowsArgumentException()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var gateService = new GateService(gateRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            gateService.AddGateAsync(new Gate { GateName = null! }));
    }

    [Test]
    public void AddGateAsync_EmptyName_ThrowsArgumentException()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var gateService = new GateService(gateRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            gateService.AddGateAsync(new Gate { GateName = string.Empty }));
    }

    [Test]
    public void AddGateAsync_WhitespaceName_ThrowsArgumentException()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var gateService = new GateService(gateRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            gateService.AddGateAsync(new Gate { GateName = " " }));
    }

    [Test]
    public async Task AddGateAsync_ValidData_CallsRepository()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        gateRepository.AddAsync(Arg.Any<Gate>()).Returns(Task.FromResult(ValidGateId));

        var gateService = new GateService(gateRepository, flightRepository);
        await gateService.AddGateAsync(new Gate { GateName = FirstGateName });

        await gateRepository.Received(1).AddAsync(Arg.Any<Gate>());
    }

    [Test]
    public async Task UpdateGateAsync_GateNotFound_DoesNotCallRepository()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        gateRepository.GetByIdAsync(ValidGateId).Returns(Task.FromResult<Gate?>(null));

        var gateService = new GateService(gateRepository, flightRepository);
        await gateService.UpdateGateAsync(new Gate { Id = ValidGateId, GateName = SecondGateName });

        await gateRepository.DidNotReceive().UpdateAsync(Arg.Any<Gate>());
    }

    [Test]
    public void UpdateGateAsync_WhitespaceNewName_ThrowsArgumentException()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        gateRepository.GetByIdAsync(ValidGateId).Returns(Task.FromResult<Gate?>(new Gate { GateName = FirstGateName }));

        var gateService = new GateService(gateRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            gateService.UpdateGateAsync(new Gate { Id = ValidGateId, GateName = " " }));
    }

    [Test]
    public void UpdateGateAsync_EmptyNewName_ThrowsArgumentException()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        gateRepository.GetByIdAsync(ValidGateId).Returns(Task.FromResult<Gate?>(new Gate { GateName = FirstGateName }));

        var gateService = new GateService(gateRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            gateService.UpdateGateAsync(new Gate { Id = ValidGateId, GateName = string.Empty }));
    }

    [Test]
    public async Task UpdateGateAsync_ValidData_UpdatesName()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var existingGate = new Gate { GateName = FirstGateName };
        gateRepository.GetByIdAsync(ValidGateId).Returns(Task.FromResult<Gate?>(existingGate));

        var gateService = new GateService(gateRepository, flightRepository);
        await gateService.UpdateGateAsync(new Gate { Id = ValidGateId, GateName = SecondGateName });

        Assert.That(existingGate.GateName, Is.EqualTo(SecondGateName));
        await gateRepository.Received(1).UpdateAsync(existingGate);
    }

    [Test]
    public async Task UpdateGateAsync_NameIsNull_CallsRepository()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var existingGate = new Gate { GateName = FirstGateName };
        gateRepository.GetByIdAsync(ValidGateId).Returns(Task.FromResult<Gate?>(existingGate));

        var gateService = new GateService(gateRepository, flightRepository);
        await gateService.UpdateGateAsync(new Gate { Id = ValidGateId, GateName = null! });

        Assert.That(existingGate.GateName, Is.EqualTo(FirstGateName));
        await gateRepository.Received(1).UpdateAsync(existingGate);
    }

    [Test]
    public async Task DeleteGateAsync_ZeroId_DoesNotCallRepository()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var gateService = new GateService(gateRepository, flightRepository);

        await gateService.DeleteGateAsync(InexistentGateId);

        await gateRepository.DidNotReceive().DeleteAsync(Arg.Any<int>());
    }

    [Test]
    public async Task DeleteGateAsync_NegativeId_DoesNotCallRepository()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var gateService = new GateService(gateRepository, flightRepository);

        await gateService.DeleteGateAsync(NegativeGateId);

        await gateRepository.DidNotReceive().DeleteAsync(Arg.Any<int>());
    }

    [Test]
    public async Task DeleteGateAsync_ValidId_CallsRepository()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        gateRepository.DeleteAsync(ValidGateId).Returns(Task.CompletedTask);

        var gateService = new GateService(gateRepository, flightRepository);
        await gateService.DeleteGateAsync(ValidGateId);

        await gateRepository.Received(1).DeleteAsync(ValidGateId);
    }

    [Test]
    public async Task SaveGateAsync_IdIsZero_CallsAdd()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        gateRepository.AddAsync(Arg.Any<Gate>()).Returns(Task.FromResult(ValidGateId));

        var gateService = new GateService(gateRepository, flightRepository);
        await gateService.SaveGateAsync(new Gate { Id = InexistentGateId, GateName = SecondGateName });

        await gateRepository.Received(1).AddAsync(Arg.Is<Gate>(addedGate => addedGate.GateName == SecondGateName));
        await gateRepository.DidNotReceive().UpdateAsync(Arg.Any<Gate>());
    }

    [Test]
    public async Task SaveGateAsync_IdIsNonZero_CallsUpdate()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var existingGate = new Gate { GateName = FirstGateName };
        gateRepository.GetByIdAsync(ValidGateId).Returns(Task.FromResult<Gate?>(existingGate));

        var gateService = new GateService(gateRepository, flightRepository);
        await gateService.SaveGateAsync(new Gate { Id = ValidGateId, GateName = SecondGateName });

        await gateRepository.Received(1).UpdateAsync(Arg.Is<Gate>(updatedGate => updatedGate.GateName == SecondGateName));
        await gateRepository.DidNotReceive().AddAsync(Arg.Any<Gate>());
    }

    [Test]
    public async Task HasFlightsAsync_FlightsExist_ReturnsTrue()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.GetByGateIdAsync(ValidGateId)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { new Flight() }));

        var gateService = new GateService(gateRepository, flightRepository);
        var result = await gateService.HasFlightsAsync(ValidGateId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasFlightsAsync_NoFlightsExist_ReturnsFalse()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.GetByGateIdAsync(ValidGateId)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var gateService = new GateService(gateRepository, flightRepository);
        var result = await gateService.HasFlightsAsync(ValidGateId);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetDeleteWarningMessageAsync_GateNotFound_ReturnsEmptyString()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        gateRepository.GetByIdAsync(InexistentGateId).Returns(Task.FromResult<Gate?>(null));

        var gateService = new GateService(gateRepository, flightRepository);
        var result = await gateService.GetDeleteWarningMessageAsync(InexistentGateId);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetDeleteWarningMessageAsync_GateHasFlights_ReturnsCriticalMessage()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var targetGate = new Gate { Id = ValidGateId, GateName = FirstGateName };
        gateRepository.GetByIdAsync(ValidGateId).Returns(Task.FromResult<Gate?>(targetGate));
        flightRepository.GetByGateIdAsync(ValidGateId)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { new Flight() }));

        var gateService = new GateService(gateRepository, flightRepository);
        var resultMessage = await gateService.GetDeleteWarningMessageAsync(ValidGateId);

        Assert.That(resultMessage, Does.Contain("CRITICAL"));
        Assert.That(resultMessage, Does.Contain(FirstGateName));
    }

    [Test]
    public async Task GetDeleteWarningMessageAsync_GateHasNoFlights_ReturnsStandardMessage()
    {
        var gateRepository = Substitute.For<IGateRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var targetGate = new Gate { Id = ValidGateId, GateName = FirstGateName };
        gateRepository.GetByIdAsync(ValidGateId).Returns(Task.FromResult<Gate?>(targetGate));
        flightRepository.GetByGateIdAsync(ValidGateId)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var gateService = new GateService(gateRepository, flightRepository);
        var resultMessage = await gateService.GetDeleteWarningMessageAsync(ValidGateId);

        Assert.That(resultMessage, Does.Not.Contain("CRITICAL"));
        Assert.That(resultMessage, Does.Contain(FirstGateName));
    }
}
