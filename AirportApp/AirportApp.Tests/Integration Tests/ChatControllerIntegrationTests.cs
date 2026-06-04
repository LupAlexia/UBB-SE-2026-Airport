using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AirportApp.Tests.IntegrationTests;

[TestFixture]
public class ChatControllerIntegrationTests
{
    private ApiTestBase _factory = null!;
    private HttpClient _client = null!;

    private const int NonExistentUserId = 999999;

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
    public async Task Post_WithNonExistentUserId_Returns404()
    {
        var requestBody = new { userId = NonExistentUserId, status = 0 };

        var response = await _client.PostAsJsonAsync("/api/chat", requestBody);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
