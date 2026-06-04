using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using AirportApp.ClassLibrary.Service.Interface;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class CompanyServiceTests
{
    private const int ValidCompanyId = 7;
    private const string ValidCompanyName = "Sky Airlines";
    private const int StartingFlightSequenceNumber = 1000;

    private static Company MakeCompany(int id = ValidCompanyId, string name = ValidCompanyName) =>
        new Company(id, name);

    private static (CompanyService service, ICompanyRepository repo, IFlightRouteService flights)
        MakeService(Company? storedCompany = null, IEnumerable<Flight>? storedFlights = null)
    {
        var repo    = Substitute.For<ICompanyRepository>();
        var flights = Substitute.For<IFlightRouteService>();
        if (storedCompany != null)
            repo.GetByIdAsync(storedCompany.Id).Returns(Task.FromResult<Company?>(storedCompany));
        if (storedFlights != null)
            flights.GetFlightsByCompanyIdAsync(ValidCompanyId)
                   .Returns(Task.FromResult<IEnumerable<Flight>>(storedFlights));
        return (new CompanyService(repo, flights), repo, flights);
    }

    [Test]
    public async Task GetCompanyByIdAsync_ZeroId_ReturnsNull()
    {
        var (service, _, _) = MakeService();
        Assert.That(await service.GetCompanyByIdAsync(0), Is.Null);
    }

    [Test]
    public async Task GetCompanyByIdAsync_NegativeId_ReturnsNull()
    {
        var (service, _, _) = MakeService();
        Assert.That(await service.GetCompanyByIdAsync(-3), Is.Null);
    }

    [Test]
    public async Task GetCompaniesByManagerIdAsync_ZeroManagerId_ReturnsEmptyCollection()
    {
        var (service, _, _) = MakeService();
        Assert.That(await service.GetCompaniesByManagerIdAsync(0), Is.Empty);
    }

    [Test]
    public async Task GetCompaniesByManagerIdAsync_NegativeManagerId_ReturnsEmptyCollection()
    {
        var (service, _, _) = MakeService();
        Assert.That(await service.GetCompaniesByManagerIdAsync(-1), Is.Empty);
    }

    [Test]
    public void AddCompanyAsync_NullName_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();
        Assert.ThrowsAsync<ArgumentException>(
            () => service.AddCompanyAsync(new Company(ValidCompanyId, null!)));
    }

    [Test]
    public void AddCompanyAsync_EmptyName_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();
        Assert.ThrowsAsync<ArgumentException>(
            () => service.AddCompanyAsync(new Company(ValidCompanyId, string.Empty)));
    }

    [Test]
    public void AddCompanyAsync_WhitespaceName_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();
        Assert.ThrowsAsync<ArgumentException>(
            () => service.AddCompanyAsync(new Company(ValidCompanyId, "   ")));
    }

    [Test]
    public async Task UpdateCompanyAsync_CompanyNotFound_DoesNotCallRepositoryUpdate()
    {
        var repo    = Substitute.For<ICompanyRepository>();
        var flights = Substitute.For<IFlightRouteService>();
        repo.GetByIdAsync(ValidCompanyId).Returns(Task.FromResult<Company?>(null));

        var service = new CompanyService(repo, flights);
        await service.UpdateCompanyAsync(MakeCompany());

        await repo.DidNotReceive().UpdateAsync(Arg.Any<Company>());
    }

    [Test]
    public void UpdateCompanyAsync_WhitespaceName_ThrowsArgumentException()
    {
        var existing = MakeCompany();
        var repo     = Substitute.For<ICompanyRepository>();
        var flights  = Substitute.For<IFlightRouteService>();
        repo.GetByIdAsync(ValidCompanyId).Returns(Task.FromResult<Company?>(existing));

        var service = new CompanyService(repo, flights);

        Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateCompanyAsync(new Company(ValidCompanyId, "   ")));
    }

    [Test]
    public async Task UpdateCompanyAsync_ValidNewName_UpdatesNameOnExistingEntity()
    {
        var existing = MakeCompany();
        var (service, repo, _) = MakeService(existing);

        await service.UpdateCompanyAsync(new Company(ValidCompanyId, "New Sky Air"));

        Assert.That(existing.Name, Is.EqualTo("New Sky Air"));
        await repo.Received(1).UpdateAsync(existing);
    }

    [Test]
    public async Task DeleteCompanyAsync_ZeroId_DoesNotCallRepository()
    {
        var (service, repo, _) = MakeService();
        await service.DeleteCompanyAsync(0);
        await repo.DidNotReceive().DeleteAsync(Arg.Any<int>());
    }

    [Test]
    public async Task DeleteCompanyAsync_NegativeId_DoesNotCallRepository()
    {
        var (service, repo, _) = MakeService();
        await service.DeleteCompanyAsync(-1);
        await repo.DidNotReceive().DeleteAsync(Arg.Any<int>());
    }

    [Test]
    public void ValidateFlightCreationInputsAsync_ZeroCompanyId_ThrowsInvalidOperationException()
    {
        var (service, _, _) = MakeService();
        Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ValidateFlightCreationInputsAsync(0, 1, "150", 1, 1));
    }

    [Test]
    public void ValidateFlightCreationInputsAsync_ZeroAirportId_ThrowsInvalidOperationException()
    {
        var (service, _, _) = MakeService();
        Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ValidateFlightCreationInputsAsync(ValidCompanyId, 0, "150", 1, 1));
    }

    [Test]
    public void ValidateFlightCreationInputsAsync_ZeroRunwayId_ThrowsInvalidOperationException()
    {
        var (service, _, _) = MakeService();
        Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ValidateFlightCreationInputsAsync(ValidCompanyId, 1, "150", 0, 1));
    }

    [Test]
    public void ValidateFlightCreationInputsAsync_ZeroGateId_ThrowsInvalidOperationException()
    {
        var (service, _, _) = MakeService();
        Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ValidateFlightCreationInputsAsync(ValidCompanyId, 1, "150", 1, 0));
    }

    [Test]
    public void ValidateFlightCreationInputsAsync_NonNumericCapacityText_ThrowsInvalidOperationException()
    {
        var (service, _, _) = MakeService();
        Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ValidateFlightCreationInputsAsync(ValidCompanyId, 1, "abc", 1, 1));
    }

    [Test]
    public async Task ValidateFlightCreationInputsAsync_ValidInputs_ReturnsParsedCapacity()
    {
        var (service, _, _) = MakeService();
        var result = await service.ValidateFlightCreationInputsAsync(ValidCompanyId, 1, "200", 1, 1);
        Assert.That(result, Is.EqualTo(200));
    }

    [Test]
    public async Task GenerateFlightCodeUsingCompanyIdAsync_TwoWordCompanyName_UsesInitialsAsPrefix()
    {
        var company = new Company(ValidCompanyId, "Sky Airlines");
        var (service, _, _) = MakeService(company, new List<Flight>());

        var code = await service.GenerateFlightCodeUsingCompanyIdAsync(ValidCompanyId);

        Assert.That(code.Split('-')[0], Is.EqualTo("SA"));
    }

    [Test]
    public async Task GenerateFlightCodeUsingCompanyIdAsync_OneWordCompanyName_UsesTwoCharPrefix()
    {
        var company = new Company(ValidCompanyId, "Ryanair");
        var (service, _, _) = MakeService(company, new List<Flight>());

        var code = await service.GenerateFlightCodeUsingCompanyIdAsync(ValidCompanyId);

        Assert.That(code, Does.StartWith("RY-"));
    }

    [Test]
    public async Task GenerateFlightCodeUsingCompanyIdAsync_NullCompany_UsesDefaultPrefix()
    {
        var repo    = Substitute.For<ICompanyRepository>();
        var flights = Substitute.For<IFlightRouteService>();
        repo.GetByIdAsync(ValidCompanyId).Returns(Task.FromResult<Company?>(null));
        flights.GetFlightsByCompanyIdAsync(ValidCompanyId)
               .Returns(Task.FromResult<IEnumerable<Flight>>(new List<Flight>()));

        var service = new CompanyService(repo, flights);
        var code    = await service.GenerateFlightCodeUsingCompanyIdAsync(ValidCompanyId);

        Assert.That(code, Does.StartWith("FL-"));
    }

    [Test]
    public async Task GenerateFlightCodeUsingCompanyIdAsync_NoExistingFlights_SequenceStartsAt1000()
    {
        var company = new Company(ValidCompanyId, "Sky Airlines");
        var (service, _, _) = MakeService(company, new List<Flight>());

        var code = await service.GenerateFlightCodeUsingCompanyIdAsync(ValidCompanyId);

        Assert.That(int.Parse(code.Split('-')[1]), Is.EqualTo(StartingFlightSequenceNumber));
    }

    [Test]
    public async Task GenerateFlightCodeUsingCompanyIdAsync_ExistingFlightsPresent_SequenceIsMaxPlusOne()
    {
        var company = new Company(ValidCompanyId, "Sky Airlines");
        var flights = new List<Flight>
        {
            new Flight { FlightNumber = "SA-1000" },
            new Flight { FlightNumber = "SA-1005" }
        };
        var (service, _, _) = MakeService(company, flights);

        var code = await service.GenerateFlightCodeUsingCompanyIdAsync(ValidCompanyId);

        Assert.That(int.Parse(code.Split('-')[1]), Is.EqualTo(1006));
    }

    [Test]
    public async Task GenerateFlightCodeUsingCompanyIdAsync_AllFlightNumbersBelowThreshold_SequenceStartsAt1000()
    {
        var company = new Company(ValidCompanyId, "Sky Airlines");
        var flights = new List<Flight>
        {
            new Flight { FlightNumber = "SA-100" },
            new Flight { FlightNumber = "SA-500" }
        };
        var (service, _, _) = MakeService(company, flights);

        var code = await service.GenerateFlightCodeUsingCompanyIdAsync(ValidCompanyId);

        Assert.That(int.Parse(code.Split('-')[1]), Is.EqualTo(StartingFlightSequenceNumber));
    }

    [Test]
    public async Task GenerateFlightCodeUsingCompanyIdAsync_MalformedFlightNumbers_TreatedAsZeroAndFallsBackTo1000()
    {
        var company = new Company(ValidCompanyId, "Sky Airlines");
        var flights = new List<Flight> { new Flight { FlightNumber = "NODELIMITER" } };
        var (service, _, _) = MakeService(company, flights);

        var code = await service.GenerateFlightCodeUsingCompanyIdAsync(ValidCompanyId);

        Assert.That(int.Parse(code.Split('-')[1]), Is.EqualTo(StartingFlightSequenceNumber));
    }
}
