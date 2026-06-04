using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class RunwayServiceTests
{
    private const int DefaultId = 1;
    private const int ValidHandleTime = 10;
    private const int UpdatedHandleTime = 30;
    private const string DefaultRunwayName = "R1";
    private const string DefaultRunwayName2 = "R2";
    private const int DefaultHandleTime = 10;
    private const int DefaultHandleTime2 = 15;
    private const string OldRunwayName = "OldName";
    private const int OldHandleTime = 5;
    private const string UpdatedRunwayName = "UpdatedName";
    private const int NonExistentId = 999;
    private const int NegativeHandleTime = -5;
    private const int NewRunwayId = 0;
    private const string NonNumericHandleTimeText = "abc";
    private const string ZeroHandleTimeText = "0";
    private const string NegativeHandleTimeText = "-5";

    [Test]
    public async Task GetAllRunwaysAsync_ReturnsAllRunways_Always()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var runways = new List<Runway>
        {
            new Runway { Name = DefaultRunwayName, HandleTime = DefaultHandleTime },
            new Runway { Name = DefaultRunwayName2, HandleTime = DefaultHandleTime2 }
        };
        runwayRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Runway>>(runways));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        var result = (await runwayService.GetAllRunwaysAsync()).ToList();

        Assert.That(result.Count, Is.EqualTo(runways.Count));
        Assert.That(result, Is.EqualTo(runways));
    }

    [Test]
    public async Task GetRunwayByIdAsync_ReturnsNull_ForNegativeId()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var runwayService = new RunwayService(runwayRepository, flightRepository);

        var result = await runwayService.GetRunwayByIdAsync(-1);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetRunwayByIdAsync_ReturnsNull_ForZeroId()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var runwayService = new RunwayService(runwayRepository, flightRepository);

        var result = await runwayService.GetRunwayByIdAsync(0);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetRunwayByIdAsync_ReturnsNull_WhenRunwayNotFound()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        runwayRepository.GetByIdAsync(NonExistentId).Returns(Task.FromResult<Runway?>(null));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        var result = await runwayService.GetRunwayByIdAsync(NonExistentId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetRunwayByIdAsync_ReturnsRunway_WhenRunwayExists()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var targetRunway = new Runway { Name = DefaultRunwayName, HandleTime = ValidHandleTime };
        runwayRepository.GetByIdAsync(DefaultId).Returns(Task.FromResult<Runway?>(targetRunway));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        var result = await runwayService.GetRunwayByIdAsync(DefaultId);

        Assert.That(result, Is.EqualTo(targetRunway));
    }

    [Test]
    public void AddRunwayAsync_ThrowsArgumentException_ForNullName()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var runwayService = new RunwayService(runwayRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            runwayService.AddRunwayAsync(new Runway { Name = null!, HandleTime = ValidHandleTime }));
    }

    [Test]
    public void AddRunwayAsync_ThrowsArgumentException_ForEmptyName()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var runwayService = new RunwayService(runwayRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            runwayService.AddRunwayAsync(new Runway { Name = string.Empty, HandleTime = ValidHandleTime }));
    }

    [Test]
    public void AddRunwayAsync_ThrowsArgumentException_ForWhitespaceName()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var runwayService = new RunwayService(runwayRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            runwayService.AddRunwayAsync(new Runway { Name = " ", HandleTime = ValidHandleTime }));
    }

    [Test]
    public void AddRunwayAsync_ThrowsArgumentException_ForInvalidHandleTime()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var runwayService = new RunwayService(runwayRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            runwayService.AddRunwayAsync(new Runway { Name = DefaultRunwayName, HandleTime = NegativeHandleTime }));
    }

    [Test]
    public async Task AddRunwayAsync_CallsRepository_WhenValidInput()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        runwayRepository.AddAsync(Arg.Any<Runway>()).Returns(Task.FromResult(DefaultId));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        await runwayService.AddRunwayAsync(new Runway { Name = DefaultRunwayName, HandleTime = ValidHandleTime });

        await runwayRepository.Received(1).AddAsync(Arg.Is<Runway>(runway =>
            runway.Name == DefaultRunwayName && runway.HandleTime == ValidHandleTime));
    }

    [Test]
    public void UpdateRunwayAsync_ThrowsInvalidOperationException_WhenRunwayNotFound()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        runwayRepository.GetByIdAsync(DefaultId).Returns(Task.FromResult<Runway?>(null));

        var runwayService = new RunwayService(runwayRepository, flightRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            runwayService.UpdateRunwayAsync(new Runway { Id = DefaultId, Name = UpdatedRunwayName }));
    }

    [Test]
    public async Task UpdateRunwayAsync_UpdatesAllFields_WhenBothAreProvided()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var existingRunway = new Runway { Name = OldRunwayName, HandleTime = OldHandleTime };
        runwayRepository.GetByIdAsync(DefaultId).Returns(Task.FromResult<Runway?>(existingRunway));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        await runwayService.UpdateRunwayAsync(new Runway { Id = DefaultId, Name = UpdatedRunwayName, HandleTime = UpdatedHandleTime });

        Assert.That(existingRunway.Name, Is.EqualTo(UpdatedRunwayName));
        Assert.That(existingRunway.HandleTime, Is.EqualTo(UpdatedHandleTime));
        await runwayRepository.Received(1).UpdateAsync(existingRunway);
    }

    [Test]
    public async Task UpdateRunwayAsync_UpdatesOnlyName_WhenHandleTimeIsZero()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var existingRunway = new Runway { Name = OldRunwayName, HandleTime = OldHandleTime };
        runwayRepository.GetByIdAsync(DefaultId).Returns(Task.FromResult<Runway?>(existingRunway));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        await runwayService.UpdateRunwayAsync(new Runway { Id = DefaultId, Name = UpdatedRunwayName, HandleTime = 0 });

        Assert.That(existingRunway.Name, Is.EqualTo(UpdatedRunwayName));
        Assert.That(existingRunway.HandleTime, Is.EqualTo(OldHandleTime));
        await runwayRepository.Received(1).UpdateAsync(existingRunway);
    }

    [Test]
    public async Task UpdateRunwayAsync_UpdatesOnlyHandleTime_WhenNameIsNull()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var existingRunway = new Runway { Name = OldRunwayName, HandleTime = OldHandleTime };
        runwayRepository.GetByIdAsync(DefaultId).Returns(Task.FromResult<Runway?>(existingRunway));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        await runwayService.UpdateRunwayAsync(new Runway { Id = DefaultId, Name = null!, HandleTime = UpdatedHandleTime });

        Assert.That(existingRunway.Name, Is.EqualTo(OldRunwayName));
        Assert.That(existingRunway.HandleTime, Is.EqualTo(UpdatedHandleTime));
        await runwayRepository.Received(1).UpdateAsync(existingRunway);
    }

    [Test]
    public void UpdateRunwayAsync_ThrowsArgumentException_ForWhitespaceName()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        runwayRepository.GetByIdAsync(DefaultId).Returns(Task.FromResult<Runway?>(new Runway { Name = OldRunwayName, HandleTime = OldHandleTime }));

        var runwayService = new RunwayService(runwayRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            runwayService.UpdateRunwayAsync(new Runway { Id = DefaultId, Name = " " }));
    }

    [Test]
    public void DeleteRunwayAsync_ThrowsInvalidOperationException_WhenRunwayNotFound()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        runwayRepository.GetByIdAsync(DefaultId).Returns(Task.FromResult<Runway?>(null));

        var runwayService = new RunwayService(runwayRepository, flightRepository);

        Assert.ThrowsAsync<InvalidOperationException>(() => runwayService.DeleteRunwayAsync(DefaultId));
    }

    [Test]
    public async Task DeleteRunwayAsync_CallsRepository_ForValidId()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        runwayRepository.GetByIdAsync(DefaultId).Returns(Task.FromResult<Runway?>(new Runway { Name = DefaultRunwayName }));
        runwayRepository.DeleteAsync(DefaultId).Returns(Task.CompletedTask);

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        await runwayService.DeleteRunwayAsync(DefaultId);

        await runwayRepository.Received(1).DeleteAsync(DefaultId);
    }

    [Test]
    public void SaveRunwayAsync_ThrowsArgumentException_ForNonNumericHandleTimeText()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var runwayService = new RunwayService(runwayRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            runwayService.SaveRunwayAsync(NewRunwayId, DefaultRunwayName, NonNumericHandleTimeText));
    }

    [Test]
    public void SaveRunwayAsync_ThrowsArgumentException_ForZeroHandleTimeText()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var runwayService = new RunwayService(runwayRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            runwayService.SaveRunwayAsync(NewRunwayId, DefaultRunwayName, ZeroHandleTimeText));
    }

    [Test]
    public void SaveRunwayAsync_ThrowsArgumentException_ForNegativeHandleTimeText()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var runwayService = new RunwayService(runwayRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            runwayService.SaveRunwayAsync(NewRunwayId, DefaultRunwayName, NegativeHandleTimeText));
    }

    [Test]
    public async Task SaveRunwayAsync_CallsAdd_WhenIdIsZero()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        runwayRepository.AddAsync(Arg.Any<Runway>()).Returns(Task.FromResult(DefaultId));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        await runwayService.SaveRunwayAsync(NewRunwayId, DefaultRunwayName, ValidHandleTime.ToString());

        await runwayRepository.Received(1).AddAsync(Arg.Any<Runway>());
        await runwayRepository.DidNotReceive().UpdateAsync(Arg.Any<Runway>());
    }

    [Test]
    public async Task SaveRunwayAsync_CallsUpdate_WhenIdIsNonZero()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        runwayRepository.GetByIdAsync(DefaultId).Returns(Task.FromResult<Runway?>(new Runway { Name = OldRunwayName, HandleTime = OldHandleTime }));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        await runwayService.SaveRunwayAsync(DefaultId, UpdatedRunwayName, UpdatedHandleTime.ToString());

        await runwayRepository.Received(1).UpdateAsync(Arg.Any<Runway>());
        await runwayRepository.DidNotReceive().AddAsync(Arg.Any<Runway>());
    }

    [Test]
    public async Task HasFlightsAsync_ReturnsTrue_WhenFlightsExist()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.GetByRunwayIdAsync(DefaultId)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { new Flight() }));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        var result = await runwayService.HasFlightsAsync(DefaultId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasFlightsAsync_ReturnsFalse_WhenNoFlightsExist()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        flightRepository.GetByRunwayIdAsync(DefaultId)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        var result = await runwayService.HasFlightsAsync(DefaultId);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetDeleteWarningMessageAsync_ReturnsEmptyString_WhenRunwayNotFound()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        runwayRepository.GetByIdAsync(0).Returns(Task.FromResult<Runway?>(null));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        var result = await runwayService.GetDeleteWarningMessageAsync(0);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetDeleteWarningMessageAsync_ReturnsCriticalMessage_WhenRunwayHasFlights()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        runwayRepository.GetByIdAsync(DefaultId).Returns(Task.FromResult<Runway?>(new Runway { Name = DefaultRunwayName }));
        flightRepository.GetByRunwayIdAsync(DefaultId)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { new Flight() }));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        var resultMessage = await runwayService.GetDeleteWarningMessageAsync(DefaultId);

        Assert.That(resultMessage, Does.Contain("CRITICAL"));
        Assert.That(resultMessage, Does.Contain(DefaultRunwayName));
    }

    [Test]
    public async Task GetDeleteWarningMessageAsync_ReturnsStandardMessage_WhenRunwayHasNoFlights()
    {
        var runwayRepository = Substitute.For<IRunwayRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        runwayRepository.GetByIdAsync(DefaultId).Returns(Task.FromResult<Runway?>(new Runway { Name = DefaultRunwayName }));
        flightRepository.GetByRunwayIdAsync(DefaultId)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var runwayService = new RunwayService(runwayRepository, flightRepository);
        var resultMessage = await runwayService.GetDeleteWarningMessageAsync(DefaultId);

        Assert.That(resultMessage, Does.Not.Contain("CRITICAL"));
        Assert.That(resultMessage, Does.Contain(DefaultRunwayName));
    }
}
