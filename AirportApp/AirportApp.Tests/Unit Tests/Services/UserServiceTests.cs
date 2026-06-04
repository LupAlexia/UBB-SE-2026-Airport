using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class UserServiceTests
{
    private const int    ValidUserId   = 1;
    private const string ValidFullName = "Alice Smith";
    private const string ValidEmail    = "alice@example.com";

    [Test]
    public void ValidateUserIntegrityAsync_NullUser_ThrowsArgumentNullException()
    {
        var service = new UserService(Substitute.For<IUserRepository>());

        Assert.ThrowsAsync<ArgumentNullException>(
            () => service.ValidateUserIntegrityAsync(null!));
    }

    [Test]
    public async Task ValidateUserIntegrityAsync_UserAlreadyInRepository_ThrowsArgumentException()
    {
        var existingUser = new User(ValidUserId, ValidFullName, ValidEmail);
        var repo         = Substitute.For<IUserRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<User>>(
            new List<User> { existingUser }));

        var service = new UserService(repo);

        Assert.ThrowsAsync<ArgumentException>(
            () => service.ValidateUserIntegrityAsync(existingUser));
    }

    [Test]
    public async Task ValidateUserIntegrityAsync_EmptyFullName_ThrowsArgumentException()
    {
        var repo = Substitute.For<IUserRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<User>>(new List<User>()));

        var service = new UserService(repo);

        Assert.ThrowsAsync<ArgumentException>(
            () => service.ValidateUserIntegrityAsync(new User(ValidUserId, string.Empty, ValidEmail)));
    }

    [Test]
    public async Task ValidateUserIntegrityAsync_EmptyEmail_ThrowsArgumentException()
    {
        var repo = Substitute.For<IUserRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<User>>(new List<User>()));

        var service = new UserService(repo);

        Assert.ThrowsAsync<ArgumentException>(
            () => service.ValidateUserIntegrityAsync(new User(ValidUserId, ValidFullName, string.Empty)));
    }

    [Test]
    public async Task ValidateUserIntegrityAsync_ValidUniqueUser_DoesNotThrow()
    {
        var repo = Substitute.For<IUserRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<User>>(new List<User>()));

        var service = new UserService(repo);

        Assert.DoesNotThrowAsync(
            () => service.ValidateUserIntegrityAsync(new User(ValidUserId, ValidFullName, ValidEmail)));
    }

    [Test]
    public async Task CreateNewUserAsync_EmptyFullName_ThrowsArgumentException()
    {
        var repo = Substitute.For<IUserRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<User>>(new List<User>()));
        repo.AddAsync(Arg.Any<User>()).Returns(Task.FromResult(0));

        var service = new UserService(repo);

        Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateNewUserAsync(ValidUserId, string.Empty, ValidEmail));
    }

    [Test]
    public async Task CreateNewUserAsync_EmptyEmail_ThrowsArgumentException()
    {
        var repo = Substitute.For<IUserRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<User>>(new List<User>()));
        repo.AddAsync(Arg.Any<User>()).Returns(Task.FromResult(0));

        var service = new UserService(repo);

        Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateNewUserAsync(ValidUserId, ValidFullName, string.Empty));
    }

    [Test]
    public void GetByIdAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        var repo = Substitute.For<IUserRepository>();
        repo.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<User?>(null));

        var service = new UserService(repo);

        Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByIdAsync(ValidUserId));
    }

    [Test]
    public async Task GetByIdAsync_UserExists_ReturnsUser()
    {
        var user = new User(ValidUserId, ValidFullName, ValidEmail);
        var repo = Substitute.For<IUserRepository>();
        repo.GetByIdAsync(ValidUserId).Returns(Task.FromResult<User?>(user));

        var service = new UserService(repo);
        var result  = await service.GetByIdAsync(ValidUserId);

        Assert.That(result, Is.SameAs(user));
    }
}
