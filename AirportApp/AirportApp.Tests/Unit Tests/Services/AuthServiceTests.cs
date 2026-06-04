using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class AuthServiceTests
{
    private const string ValidEmail    = "user@example.com";
    private const string ValidPhone    = "1234567890";
    private const string ValidUsername = "alice";
    private const string ValidPassword = "password123";
    private const int    ExistingUserId = 42;

    private static Customer MakeHashedCustomer(string email, string rawPassword)
    {
        var c = new Customer { Email = email, Username = ValidUsername, Id = ExistingUserId };
        c.PasswordHash = new PasswordHasher<Customer>().HashPassword(c, rawPassword);
        return c;
    }

    [Test]
    public void LoginAsync_NullEmail_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(() => service.LoginAsync(null!, ValidPassword));
    }

    [Test]
    public void LoginAsync_WhitespaceEmail_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(() => service.LoginAsync("   ", ValidPassword));
    }

    [Test]
    public void LoginAsync_NullPassword_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(() => service.LoginAsync(ValidEmail, null!));
    }

    [Test]
    public void LoginAsync_EmptyPassword_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(() => service.LoginAsync(ValidEmail, string.Empty));
    }

    [Test]
    public void LoginAsync_UnknownEmail_ThrowsInvalidOperationException()
    {
        var repo = Substitute.For<ICustomerRepository>();
        repo.GetByEmailAsync(Arg.Any<string>()).Returns(Task.FromResult<Customer?>(null));

        var service = new AuthService(repo);

        Assert.ThrowsAsync<InvalidOperationException>(() => service.LoginAsync(ValidEmail, ValidPassword));
    }

    [Test]
    public void LoginAsync_WrongPassword_ThrowsInvalidOperationException()
    {
        var repo     = Substitute.For<ICustomerRepository>();
        var customer = MakeHashedCustomer(ValidEmail, ValidPassword);
        repo.GetByEmailAsync(ValidEmail).Returns(Task.FromResult<Customer?>(customer));

        var service = new AuthService(repo);

        Assert.ThrowsAsync<InvalidOperationException>(() => service.LoginAsync(ValidEmail, "wrong!"));
    }

    [Test]
    public async Task LoginAsync_CorrectCredentials_ReturnsMatchingCustomer()
    {
        var repo     = Substitute.For<ICustomerRepository>();
        var customer = MakeHashedCustomer(ValidEmail, ValidPassword);
        repo.GetByEmailAsync(ValidEmail).Returns(Task.FromResult<Customer?>(customer));

        var service = new AuthService(repo);
        var result  = await service.LoginAsync(ValidEmail, ValidPassword);

        Assert.That(result, Is.SameAs(customer));
    }

    [Test]
    public async Task LoginAsync_EmailWithSurroundingWhitespace_TrimsAndSucceeds()
    {
        var repo     = Substitute.For<ICustomerRepository>();
        var customer = MakeHashedCustomer(ValidEmail, ValidPassword);
        repo.GetByEmailAsync(ValidEmail).Returns(Task.FromResult<Customer?>(customer));

        var service = new AuthService(repo);
        var result  = await service.LoginAsync("  " + ValidEmail + "  ", ValidPassword);

        Assert.That(result, Is.SameAs(customer));
    }

    [Test]
    public void LoginAsync_CurrentUserIdMismatch_ThrowsInvalidOperationException()
    {
        var repo     = Substitute.For<ICustomerRepository>();
        var customer = MakeHashedCustomer(ValidEmail, ValidPassword);
        repo.GetByEmailAsync(ValidEmail).Returns(Task.FromResult<Customer?>(customer));

        var service = new AuthService(repo);

        Assert.ThrowsAsync<InvalidOperationException>(
            () => service.LoginAsync(ValidEmail, ValidPassword, currentUserId: 999));
    }

    [Test]
    public async Task LoginAsync_CurrentUserIdMatchesCustomer_ReturnsCustomer()
    {
        var repo     = Substitute.For<ICustomerRepository>();
        var customer = MakeHashedCustomer(ValidEmail, ValidPassword);
        repo.GetByEmailAsync(ValidEmail).Returns(Task.FromResult<Customer?>(customer));

        var service = new AuthService(repo);
        var result  = await service.LoginAsync(ValidEmail, ValidPassword, currentUserId: ExistingUserId);

        Assert.That(result, Is.SameAs(customer));
    }

    [Test]
    public void RegisterAsync_NullEmail_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(null!, ValidPhone, ValidUsername, ValidPassword));
    }

    [Test]
    public void RegisterAsync_WhitespaceEmail_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync("   ", ValidPhone, ValidUsername, ValidPassword));
    }

    [Test]
    public void RegisterAsync_InvalidEmailFormat_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync("notanemail", ValidPhone, ValidUsername, ValidPassword));
    }

    [Test]
    public void RegisterAsync_NullUsername_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(ValidEmail, ValidPhone, null!, ValidPassword));
    }

    [Test]
    public void RegisterAsync_WhitespaceUsername_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(ValidEmail, ValidPhone, "   ", ValidPassword));
    }

    [Test]
    public void RegisterAsync_UsernameShorterThanMinimum_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(ValidEmail, ValidPhone, "ab", ValidPassword));
    }

    [Test]
    public void RegisterAsync_UsernameWithInvalidCharacters_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(ValidEmail, ValidPhone, "ali!ce", ValidPassword));
    }

    [Test]
    public void RegisterAsync_NullPhone_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(ValidEmail, null!, ValidUsername, ValidPassword));
    }

    [Test]
    public void RegisterAsync_WhitespacePhone_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(ValidEmail, "   ", ValidUsername, ValidPassword));
    }

    [Test]
    public void RegisterAsync_PhoneContainsLetters_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(ValidEmail, "12345abc90", ValidUsername, ValidPassword));
    }

    [Test]
    public void RegisterAsync_PhoneTooShort_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(ValidEmail, "123456789", ValidUsername, ValidPassword));
    }

    [Test]
    public void RegisterAsync_PhoneTooLong_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(ValidEmail, "1234567890123456", ValidUsername, ValidPassword));
    }

    [Test]
    public void RegisterAsync_NullPassword_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(ValidEmail, ValidPhone, ValidUsername, null!));
    }

    [Test]
    public void RegisterAsync_PasswordShorterThanMinimum_ThrowsArgumentException()
    {
        var service = new AuthService(Substitute.For<ICustomerRepository>());

        Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(ValidEmail, ValidPhone, ValidUsername, "abc12"));
    }

    [Test]
    public void RegisterAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var repo = Substitute.For<ICustomerRepository>();
        repo.GetByEmailAsync(ValidEmail).Returns(
            Task.FromResult<Customer?>(new Customer { Email = ValidEmail }));

        var service = new AuthService(repo);

        Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegisterAsync(ValidEmail, ValidPhone, ValidUsername, ValidPassword));
    }

    [Test]
    public async Task RegisterAsync_ValidInputs_StoresHashedPasswordNotPlainText()
    {
        var repo = Substitute.For<ICustomerRepository>();
        repo.GetByEmailAsync(ValidEmail).Returns(Task.FromResult<Customer?>(null));

        var service = new AuthService(repo);
        await service.RegisterAsync(ValidEmail, ValidPhone, ValidUsername, ValidPassword);

        await repo.Received(1).AddAsync(Arg.Is<Customer>(c =>
            !string.IsNullOrEmpty(c.PasswordHash) && c.PasswordHash != ValidPassword));
    }

    [Test]
    public void GetByIdAsync_CustomerNotFound_ThrowsKeyNotFoundException()
    {
        var repo = Substitute.For<ICustomerRepository>();
        repo.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Customer?>(null));

        var service = new AuthService(repo);

        Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByIdAsync(ExistingUserId));
    }

    [Test]
    public async Task GetByIdAsync_CustomerExists_ReturnsCustomer()
    {
        var customer = new Customer { Id = ExistingUserId, Email = ValidEmail };
        var repo     = Substitute.For<ICustomerRepository>();
        repo.GetByIdAsync(ExistingUserId).Returns(Task.FromResult<Customer?>(customer));

        var service = new AuthService(repo);
        var result  = await service.GetByIdAsync(ExistingUserId);

        Assert.That(result, Is.SameAs(customer));
    }
}
