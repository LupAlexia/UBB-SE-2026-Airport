using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirportApp.Tests.IntegrationTests;

[TestFixture]
public class FlightRoutesControllerIntegrationTests
{
    private ApiTestBase _factory = null!;
    private HttpClient _client = null!;

    private const int NonExistentFlightId = 999999;
    private const int NonExistentRouteId = 999999;

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
    public async Task GetFlightById_WithNonExistentId_Returns404()
    {
        var response = await _client.GetAsync($"/api/flight-routes/flights/{NonExistentFlightId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteFlight_WithNonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync($"/api/flight-routes/flights/{NonExistentFlightId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetRouteById_WithNonExistentId_Returns404()
    {
        var response = await _client.GetAsync($"/api/flight-routes/routes/{NonExistentRouteId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
