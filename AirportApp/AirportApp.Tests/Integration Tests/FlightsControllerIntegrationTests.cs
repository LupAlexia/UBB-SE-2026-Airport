using AirportApp.Api.Controllers;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AirportApp.Tests.IntegrationTests;

[TestFixture]
public class FlightsControllerIntegrationTests
{
    private ApiTestBase _factory = null!;
    private HttpClient _client = null!;

    private const int NonExistentFlightId = 999999;
    private const string NonExistentFlightNumber = "FL-GHOST";
    private const int PlaceholderRouteId = 1;
    private const int PlaceholderRunwayId = 1;
    private const int PlaceholderGateId = 1;

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
    public async Task Put_WithNonExistentFlightId_Returns404()
    {
        var requestBody = new FlightRequestDTO(
            NonExistentFlightNumber,
            PlaceholderRouteId,
            DateTime.UtcNow,
            PlaceholderRunwayId,
            PlaceholderGateId);

        var response = await _client.PutAsJsonAsync($"/api/flights/{NonExistentFlightId}", requestBody);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Delete_WithNonExistentFlightId_Returns404()
    {
        var response = await _client.DeleteAsync($"/api/flights/{NonExistentFlightId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
