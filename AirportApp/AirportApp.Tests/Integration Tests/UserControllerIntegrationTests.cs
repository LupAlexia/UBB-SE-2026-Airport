using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirportApp.Tests.IntegrationTests;

[TestFixture]
public class UserControllerIntegrationTests
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
    public async Task GetUserById_WithNonExistentId_Returns404()
    {
        // Arrange
        int nonExistentId = 999999;

        // Act
        var response = await _client.GetAsync($"/api/user/{nonExistentId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
