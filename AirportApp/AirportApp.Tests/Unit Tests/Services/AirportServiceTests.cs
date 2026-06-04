using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class AirportServiceTests
{
    private const int TargetAirportId = 1;
    private const int InvalidAirportId = 0;
    private const int NegativeAirportId = -1;
    private const string DefaultTestCode = "DTC";
    private const string DefaultTestName = "DefaultTestName";
    private const string DefaultTestCity = "DefaultTestCity";
    private const string SecondTestCode = "STC";
    private const string SecondTestName = "SecondTestName";
    private const string SecondTestCity = "SecondTestCity";
    private const string UpdatedName = "New Name";
    private const string UpdatedCity = "New City";
    private const string UpdatedCode = "NEW";

    [Test]
    public void AddAirportAsync_CodeIsNull_ThrowsArgumentException()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var airportService = new AirportService(airportRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            airportService.AddAirportAsync(new Airport { AirportCode = null!, Name = DefaultTestName, City = DefaultTestCity }));
    }

    [Test]
    public void AddAirportAsync_CodeIsWhitespace_ThrowsArgumentException()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var airportService = new AirportService(airportRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            airportService.AddAirportAsync(new Airport { AirportCode = " ", Name = DefaultTestName, City = DefaultTestCity }));
    }

    [Test]
    public void AddAirportAsync_NameIsNull_ThrowsArgumentException()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var airportService = new AirportService(airportRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            airportService.AddAirportAsync(new Airport { AirportCode = DefaultTestCode, Name = null!, City = DefaultTestCity }));
    }

    [Test]
    public void AddAirportAsync_NameIsWhitespace_ThrowsArgumentException()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var airportService = new AirportService(airportRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            airportService.AddAirportAsync(new Airport { AirportCode = DefaultTestCode, Name = " ", City = DefaultTestCity }));
    }

    [Test]
    public void AddAirportAsync_CityIsNull_ThrowsArgumentException()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var airportService = new AirportService(airportRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            airportService.AddAirportAsync(new Airport { AirportCode = DefaultTestCode, Name = DefaultTestName, City = null! }));
    }

    [Test]
    public void AddAirportAsync_CityIsWhitespace_ThrowsArgumentException()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var airportService = new AirportService(airportRepository, flightRepository);

        Assert.ThrowsAsync<ArgumentException>(() =>
            airportService.AddAirportAsync(new Airport { AirportCode = DefaultTestCode, Name = DefaultTestName, City = " " }));
    }

    [Test]
    public async Task AddAirportAsync_ArgumentsAreValid_CallsRepository()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        airportRepository.AddAsync(Arg.Any<Airport>()).Returns(Task.FromResult(TargetAirportId));

        var airportService = new AirportService(airportRepository, flightRepository);
        await airportService.AddAirportAsync(new Airport { AirportCode = DefaultTestCode, Name = DefaultTestName, City = DefaultTestCity });

        await airportRepository.Received(1).AddAsync(Arg.Any<Airport>());
    }

    [Test]
    public async Task GetAirportByIdAsync_IdIsInvalid_ReturnsNull()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var airportService = new AirportService(airportRepository, flightRepository);

        var result = await airportService.GetAirportByIdAsync(InvalidAirportId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAirportByIdAsync_IdIsValid_ReturnsAirportObject()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var existingAirport = new Airport { Id = TargetAirportId, AirportCode = DefaultTestCode };
        airportRepository.GetByIdAsync(TargetAirportId).Returns(Task.FromResult<Airport?>(existingAirport));

        var airportService = new AirportService(airportRepository, flightRepository);
        var result = await airportService.GetAirportByIdAsync(TargetAirportId);

        Assert.That(result, Is.EqualTo(existingAirport));
    }

    [Test]
    public async Task UpdateAirportAsync_AirportNotFound_DoesNotCallRepository()
    {
        var airportRepositoryThatReturnsNull = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        airportRepositoryThatReturnsNull.GetByIdAsync(TargetAirportId).Returns(Task.FromResult<Airport?>(null));

        var airportService = new AirportService(airportRepositoryThatReturnsNull, flightRepository);
        await airportService.UpdateAirportAsync(new Airport { Id = TargetAirportId, City = DefaultTestCity });

        await airportRepositoryThatReturnsNull.DidNotReceive().UpdateAsync(Arg.Any<Airport>());
    }

    [Test]
    public async Task UpdateAirportAsync_OtherFieldsAreNull_UpdatesOnlyName()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var airportToUpdate = new Airport { AirportCode = DefaultTestCode, Name = DefaultTestName, City = DefaultTestCity };
        airportRepository.GetByIdAsync(TargetAirportId).Returns(Task.FromResult<Airport?>(airportToUpdate));

        var airportService = new AirportService(airportRepository, flightRepository);
        await airportService.UpdateAirportAsync(new Airport { Id = TargetAirportId, Name = UpdatedName, AirportCode = null!, City = null! });

        Assert.That(airportToUpdate.AirportCode, Is.EqualTo(DefaultTestCode));
        Assert.That(airportToUpdate.Name, Is.EqualTo(UpdatedName));
        Assert.That(airportToUpdate.City, Is.EqualTo(DefaultTestCity));
        await airportRepository.Received(1).UpdateAsync(airportToUpdate);
    }

    [Test]
    public async Task UpdateAirportAsync_OtherFieldsAreNull_UpdatesOnlyCity()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var airportToUpdate = new Airport { AirportCode = DefaultTestCode, Name = DefaultTestName, City = DefaultTestCity };
        airportRepository.GetByIdAsync(TargetAirportId).Returns(Task.FromResult<Airport?>(airportToUpdate));

        var airportService = new AirportService(airportRepository, flightRepository);
        await airportService.UpdateAirportAsync(new Airport { Id = TargetAirportId, City = UpdatedCity, AirportCode = null!, Name = null! });

        Assert.That(airportToUpdate.AirportCode, Is.EqualTo(DefaultTestCode));
        Assert.That(airportToUpdate.Name, Is.EqualTo(DefaultTestName));
        Assert.That(airportToUpdate.City, Is.EqualTo(UpdatedCity));
        await airportRepository.Received(1).UpdateAsync(airportToUpdate);
    }

    [Test]
    public async Task UpdateAirportAsync_OtherFieldsAreNull_UpdatesOnlyCode()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var airportToUpdate = new Airport { AirportCode = DefaultTestCode, Name = DefaultTestName, City = DefaultTestCity };
        airportRepository.GetByIdAsync(TargetAirportId).Returns(Task.FromResult<Airport?>(airportToUpdate));

        var airportService = new AirportService(airportRepository, flightRepository);
        await airportService.UpdateAirportAsync(new Airport { Id = TargetAirportId, AirportCode = UpdatedCode, Name = null!, City = null! });

        Assert.That(airportToUpdate.AirportCode, Is.EqualTo(UpdatedCode));
        Assert.That(airportToUpdate.Name, Is.EqualTo(DefaultTestName));
        Assert.That(airportToUpdate.City, Is.EqualTo(DefaultTestCity));
        await airportRepository.Received(1).UpdateAsync(airportToUpdate);
    }

    [Test]
    public async Task UpdateAirportAsync_AllFieldsAreProvided_UpdatesAllFields()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var airportToUpdate = new Airport { AirportCode = DefaultTestCode, Name = DefaultTestName, City = DefaultTestCity };
        airportRepository.GetByIdAsync(TargetAirportId).Returns(Task.FromResult<Airport?>(airportToUpdate));

        var airportService = new AirportService(airportRepository, flightRepository);
        await airportService.UpdateAirportAsync(new Airport { Id = TargetAirportId, AirportCode = UpdatedCode, Name = UpdatedName, City = UpdatedCity });

        Assert.That(airportToUpdate.AirportCode, Is.EqualTo(UpdatedCode));
        Assert.That(airportToUpdate.Name, Is.EqualTo(UpdatedName));
        Assert.That(airportToUpdate.City, Is.EqualTo(UpdatedCity));
        await airportRepository.Received(1).UpdateAsync(airportToUpdate);
    }

    [Test]
    public async Task UpdateAirportAsync_NoChangesProvided_CallsRepository()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var airportToUpdate = new Airport { AirportCode = DefaultTestCode, Name = DefaultTestName, City = DefaultTestCity };
        airportRepository.GetByIdAsync(TargetAirportId).Returns(Task.FromResult<Airport?>(airportToUpdate));

        var airportService = new AirportService(airportRepository, flightRepository);
        await airportService.UpdateAirportAsync(new Airport { Id = TargetAirportId, AirportCode = DefaultTestCode, Name = DefaultTestName, City = DefaultTestCity });

        Assert.That(airportToUpdate.AirportCode, Is.EqualTo(DefaultTestCode));
        Assert.That(airportToUpdate.Name, Is.EqualTo(DefaultTestName));
        Assert.That(airportToUpdate.City, Is.EqualTo(DefaultTestCity));
        await airportRepository.Received(1).UpdateAsync(airportToUpdate);
    }

    [Test]
    public async Task DeleteAirportAsync_IdIsInvalid_DoesNotCallRepository()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var airportService = new AirportService(airportRepository, flightRepository);

        await airportService.DeleteAirportAsync(InvalidAirportId);

        await airportRepository.DidNotReceive().DeleteAsync(Arg.Any<int>());
    }

    [Test]
    public async Task DeleteAirportAsync_IdIsValid_CallsRepositoryDelete()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        airportRepository.DeleteAsync(TargetAirportId).Returns(Task.CompletedTask);

        var airportService = new AirportService(airportRepository, flightRepository);
        await airportService.DeleteAirportAsync(TargetAirportId);

        await airportRepository.Received(1).DeleteAsync(TargetAirportId);
    }

    [Test]
    public async Task HasFlightsAsync_AssociatedFlightsExist_ReturnsTrue()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepositoryWithFlights = Substitute.For<IFlightRepository>();
        flightRepositoryWithFlights.GetByAirportIdAsync(TargetAirportId)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { new Flight() }));

        var airportService = new AirportService(airportRepository, flightRepositoryWithFlights);
        var result = await airportService.HasFlightsAsync(TargetAirportId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasFlightsAsync_NoAssociatedFlightsFound_ReturnsFalse()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepositoryWithNoFlights = Substitute.For<IFlightRepository>();
        flightRepositoryWithNoFlights.GetByAirportIdAsync(TargetAirportId)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var airportService = new AirportService(airportRepository, flightRepositoryWithNoFlights);
        var result = await airportService.HasFlightsAsync(TargetAirportId);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task SaveAirportAsync_IdIsZero_CallsAdd()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        airportRepository.AddAsync(Arg.Any<Airport>()).Returns(Task.FromResult(TargetAirportId));

        var airportService = new AirportService(airportRepository, flightRepository);
        await airportService.SaveAirportAsync(new Airport { Id = 0, AirportCode = DefaultTestCode, Name = DefaultTestName, City = DefaultTestCity });

        await airportRepository.Received(1).AddAsync(Arg.Any<Airport>());
        await airportRepository.DidNotReceive().UpdateAsync(Arg.Any<Airport>());
    }

    [Test]
    public async Task SaveAirportAsync_IdIsNonZero_CallsUpdate()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var existingAirport = new Airport { Id = TargetAirportId, AirportCode = DefaultTestCode, Name = DefaultTestName, City = DefaultTestCity };
        airportRepository.GetByIdAsync(TargetAirportId).Returns(Task.FromResult<Airport?>(existingAirport));

        var airportService = new AirportService(airportRepository, flightRepository);
        await airportService.SaveAirportAsync(new Airport { Id = TargetAirportId, AirportCode = UpdatedCode, Name = UpdatedName, City = UpdatedCity });

        await airportRepository.Received(1).UpdateAsync(Arg.Any<Airport>());
        await airportRepository.DidNotReceive().AddAsync(Arg.Any<Airport>());
    }

    [Test]
    public async Task GetDeleteWarningMessageAsync_AirportHasFlights_ReturnsCriticalMessage()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var targetAirport = new Airport { Id = TargetAirportId, Name = DefaultTestName };
        airportRepository.GetByIdAsync(TargetAirportId).Returns(Task.FromResult<Airport?>(targetAirport));
        flightRepository.GetByAirportIdAsync(TargetAirportId)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight> { new Flight() }));

        var airportService = new AirportService(airportRepository, flightRepository);
        var resultMessage = await airportService.GetDeleteWarningMessageAsync(TargetAirportId);

        Assert.That(resultMessage, Does.Contain("CRITICAL"));
        Assert.That(resultMessage, Does.Contain(DefaultTestName));
    }

    [Test]
    public async Task GetDeleteWarningMessageAsync_AirportHasNoFlights_ReturnsStandardMessage()
    {
        var airportRepository = Substitute.For<IAirportRepository>();
        var flightRepository = Substitute.For<IFlightRepository>();
        var targetAirport = new Airport { Id = TargetAirportId, Name = DefaultTestName };
        airportRepository.GetByIdAsync(TargetAirportId).Returns(Task.FromResult<Airport?>(targetAirport));
        flightRepository.GetByAirportIdAsync(TargetAirportId)
            .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var airportService = new AirportService(airportRepository, flightRepository);
        var resultMessage = await airportService.GetDeleteWarningMessageAsync(TargetAirportId);

        Assert.That(resultMessage, Does.Not.Contain("CRITICAL"));
        Assert.That(resultMessage, Does.Contain(DefaultTestName));
    }
}
