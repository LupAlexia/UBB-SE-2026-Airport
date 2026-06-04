using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirportApp.Tests.IntegrationTests;

[TestFixture]
public class TicketControllerIntegrationTests
{
    private ApiTestBase _factory = null!;
    private HttpClient _client = null!;

    private const int NonExistentTicketId = 999999;
    private const int AnyShopId = 1;

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
    public async Task GetByShop_WithAnyShopId_Returns200()
    {
        var response = await _client.GetAsync($"/api/ticket/by-shop/{AnyShopId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetCountByShop_WithAnyShopId_Returns200()
    {
        var response = await _client.GetAsync($"/api/ticket/count/by-shop/{AnyShopId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetById_WithNonExistentId_Returns404()
    {
        var response = await _client.GetAsync($"/api/ticket/{NonExistentTicketId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
