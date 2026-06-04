using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class ClientServiceTests
{
    private const int    ValidClientId   = 1;
    private const string ValidClientName = "Acme Corp";

    [Test]
    public void AddClientAsync_NullName_ThrowsArgumentException()
    {
        var service = new ClientService(Substitute.For<IClientRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.AddClientAsync(new Client(ValidClientId, null!)));
    }

    [Test]
    public void AddClientAsync_EmptyName_ThrowsArgumentException()
    {
        var service = new ClientService(Substitute.For<IClientRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.AddClientAsync(new Client(ValidClientId, string.Empty)));
    }

    [Test]
    public void AddClientAsync_WhitespaceName_ThrowsArgumentException()
    {
        var service = new ClientService(Substitute.For<IClientRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.AddClientAsync(new Client(ValidClientId, "   ")));
    }

    [Test]
    public void UpdateClientAsync_NullName_ThrowsArgumentException()
    {
        var service = new ClientService(Substitute.For<IClientRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateClientAsync(new Client(ValidClientId, null!)));
    }

    [Test]
    public void UpdateClientAsync_EmptyName_ThrowsArgumentException()
    {
        var service = new ClientService(Substitute.For<IClientRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateClientAsync(new Client(ValidClientId, string.Empty)));
    }

    [Test]
    public void UpdateClientAsync_WhitespaceName_ThrowsArgumentException()
    {
        var service = new ClientService(Substitute.For<IClientRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateClientAsync(new Client(ValidClientId, "   ")));
    }

    [Test]
    public void GetAnyClientAsync_NoClientsInRepository_ThrowsInvalidOperationException()
    {
        var repo = Substitute.For<IClientRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Client>>(new List<Client>()));

        var service = new ClientService(repo);

        Assert.ThrowsAsync<InvalidOperationException>(() => service.GetAnyClientAsync());
    }

    [Test]
    public async Task GetAnyClientAsync_MultipleClientsInRepository_ReturnsFirstClient()
    {
        var first  = new Client(1, "First Corp");
        var second = new Client(2, "Second Corp");

        var repo = Substitute.For<IClientRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Client>>(
            new List<Client> { first, second }));

        var result = await new ClientService(repo).GetAnyClientAsync();

        Assert.That(result, Is.SameAs(first));
    }
}
