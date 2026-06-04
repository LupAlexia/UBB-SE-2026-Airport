using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class ManagerServiceTests
{
    private const int    ValidManagerId = 3;
    private const string ValidName     = "Charlie Manager";
    private const string ValidEmail    = "charlie@airport.com";
    private const string ValidPhone    = "0712345678";

    private static Manager MakeManager(
        int    id    = ValidManagerId,
        string name  = ValidName,
        string email = ValidEmail,
        string phone = ValidPhone) =>
        new Manager(id, name, email, phone);

    [Test]
    public void AddManagerAsync_NullName_ThrowsArgumentException()
    {
        var service = new ManagerService(Substitute.For<IManagerRepository>());
        Assert.ThrowsAsync<ArgumentException>(() => service.AddManagerAsync(MakeManager(name: null!)));
    }

    [Test]
    public void AddManagerAsync_WhitespaceName_ThrowsArgumentException()
    {
        var service = new ManagerService(Substitute.For<IManagerRepository>());
        Assert.ThrowsAsync<ArgumentException>(() => service.AddManagerAsync(MakeManager(name: "   ")));
    }

    [Test]
    public void AddManagerAsync_NullEmail_ThrowsArgumentException()
    {
        var service = new ManagerService(Substitute.For<IManagerRepository>());
        Assert.ThrowsAsync<ArgumentException>(() => service.AddManagerAsync(MakeManager(email: null!)));
    }

    [Test]
    public void AddManagerAsync_WhitespaceEmail_ThrowsArgumentException()
    {
        var service = new ManagerService(Substitute.For<IManagerRepository>());
        Assert.ThrowsAsync<ArgumentException>(() => service.AddManagerAsync(MakeManager(email: "   ")));
    }

    [Test]
    public void AddManagerAsync_EmailWithoutAtSign_ThrowsArgumentException()
    {
        var service = new ManagerService(Substitute.For<IManagerRepository>());
        Assert.ThrowsAsync<ArgumentException>(
            () => service.AddManagerAsync(MakeManager(email: "notanemail")));
    }

    [Test]
    public void AddManagerAsync_NullPhone_ThrowsArgumentException()
    {
        var service = new ManagerService(Substitute.For<IManagerRepository>());
        Assert.ThrowsAsync<ArgumentException>(() => service.AddManagerAsync(MakeManager(phone: null!)));
    }

    [Test]
    public void AddManagerAsync_WhitespacePhone_ThrowsArgumentException()
    {
        var service = new ManagerService(Substitute.For<IManagerRepository>());
        Assert.ThrowsAsync<ArgumentException>(() => service.AddManagerAsync(MakeManager(phone: "   ")));
    }

    [Test]
    public void UpdateManagerAsync_NullName_ThrowsArgumentException()
    {
        var service = new ManagerService(Substitute.For<IManagerRepository>());
        Assert.ThrowsAsync<ArgumentException>(() => service.UpdateManagerAsync(MakeManager(name: null!)));
    }

    [Test]
    public void UpdateManagerAsync_WhitespaceName_ThrowsArgumentException()
    {
        var service = new ManagerService(Substitute.For<IManagerRepository>());
        Assert.ThrowsAsync<ArgumentException>(() => service.UpdateManagerAsync(MakeManager(name: "   ")));
    }

    [Test]
    public void UpdateManagerAsync_NullEmail_ThrowsArgumentException()
    {
        var service = new ManagerService(Substitute.For<IManagerRepository>());
        Assert.ThrowsAsync<ArgumentException>(() => service.UpdateManagerAsync(MakeManager(email: null!)));
    }

    [Test]
    public void UpdateManagerAsync_EmailWithoutAtSign_ThrowsArgumentException()
    {
        var service = new ManagerService(Substitute.For<IManagerRepository>());
        Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateManagerAsync(MakeManager(email: "invalidemail")));
    }

    [Test]
    public void UpdateManagerAsync_WhitespacePhone_ThrowsArgumentException()
    {
        var service = new ManagerService(Substitute.For<IManagerRepository>());
        Assert.ThrowsAsync<ArgumentException>(() => service.UpdateManagerAsync(MakeManager(phone: "   ")));
    }

    [Test]
    public async Task GetAnyManagerAsync_EmptyRepository_ReturnsNull()
    {
        var repo = Substitute.For<IManagerRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Manager>>(new List<Manager>()));

        var result = await new ManagerService(repo).GetAnyManagerAsync();

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAnyManagerAsync_MultipleManagers_ReturnsFirstManager()
    {
        var first  = MakeManager(id: 1, name: "First");
        var second = MakeManager(id: 2, name: "Second");

        var repo = Substitute.For<IManagerRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Manager>>(
            new List<Manager> { first, second }));

        var result = await new ManagerService(repo).GetAnyManagerAsync();

        Assert.That(result, Is.SameAs(first));
    }
}
