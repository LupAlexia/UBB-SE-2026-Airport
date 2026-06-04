using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class AdministratorServiceTests
{
    private const int    ValidAdminId = 10;
    private const string ValidName   = "Admin Alice";
    private const string ValidEmail  = "alice@airport.com";

    [Test]
    public void ValidateAdministratorIntegrityAsync_NullEntity_ThrowsArgumentNullException()
    {
        var service = new AdministratorService(Substitute.For<IAdministratorRepository>());

        Assert.ThrowsAsync<ArgumentNullException>(
            () => service.ValidateAdministratorIntegrityAsync(null!));
    }

    [Test]
    public async Task ValidateAdministratorIntegrityAsync_DuplicateEntity_ThrowsArgumentException()
    {
        var existing = new Administrator(ValidAdminId, ValidName, ValidEmail);
        var repo     = Substitute.For<IAdministratorRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Administrator>>(
            new List<Administrator> { existing }));

        var service = new AdministratorService(repo);

        Assert.ThrowsAsync<ArgumentException>(
            () => service.ValidateAdministratorIntegrityAsync(existing));
    }

    [Test]
    public async Task ValidateAdministratorIntegrityAsync_EmptyFullName_ThrowsArgumentException()
    {
        var repo = Substitute.For<IAdministratorRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Administrator>>(new List<Administrator>()));

        var service = new AdministratorService(repo);

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.ValidateAdministratorIntegrityAsync(
                new Administrator(ValidAdminId, string.Empty, ValidEmail)));
    }

    [Test]
    public async Task ValidateAdministratorIntegrityAsync_EmptyEmail_ThrowsArgumentException()
    {
        var repo = Substitute.For<IAdministratorRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Administrator>>(new List<Administrator>()));

        var service = new AdministratorService(repo);

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.ValidateAdministratorIntegrityAsync(
                new Administrator(ValidAdminId, ValidName, string.Empty)));
    }

    [Test]
    public async Task ValidateAdministratorIntegrityAsync_ValidUniqueEntity_DoesNotThrow()
    {
        var repo = Substitute.For<IAdministratorRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Administrator>>(new List<Administrator>()));

        var service = new AdministratorService(repo);

        Assert.DoesNotThrowAsync(() =>
            service.ValidateAdministratorIntegrityAsync(
                new Administrator(ValidAdminId, ValidName, ValidEmail)));
    }
}
