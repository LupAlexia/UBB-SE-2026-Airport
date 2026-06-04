using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AirportApp.Tests.IntegrationTests;

[TestFixture]
public class AuthControllerIntegrationTests
{
    private ApiTestBase _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new ApiTestBase();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task Login_WithValidCredentials_Returns200()
    {
        // Arrange
        var customerEmail = "integrationtestuser@example.com";
        var password = "TestPassword123!";
        
        await _factory.ExecuteDbContextAsync(async db =>
        {
            var customer = new Customer
            {
                Email = customerEmail,
                Username = "testuser",
                Phone = "0740123456"
            };
            var passwordHasher = new PasswordHasher<Customer>();
            customer.PasswordHash = passwordHasher.HashPassword(customer, password);

            db.Customers.Add(customer);
            await db.SaveChangesAsync();
        });

        var requestDto = new UnifiedLoginRequestDTO
        {
            Role = "customer",
            Email = customerEmail,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", requestDto);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var customerEmail = "integrationtestuser_token@example.com";
        var password = "TestPassword123!";
        
        await _factory.ExecuteDbContextAsync(async db =>
        {
            var customer = new Customer
            {
                Email = customerEmail,
                Username = "testuser_token",
                Phone = "0740123456"
            };
            var passwordHasher = new PasswordHasher<Customer>();
            customer.PasswordHash = passwordHasher.HashPassword(customer, password);

            db.Customers.Add(customer);
            await db.SaveChangesAsync();
        });

        var requestDto = new UnifiedLoginRequestDTO
        {
            Role = "customer",
            Email = customerEmail,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", requestDto);

        // Assert
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
        Assert.That(loginResponse?.Token, Is.EqualTo("mock-jwt-token"));
    }

    [Test]
    public async Task Login_WithInvalidCredentials_Returns401()
    {
        // Arrange
        var requestDto = new UnifiedLoginRequestDTO
        {
            Role = "customer",
            Email = "nonexistent@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", requestDto);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}
