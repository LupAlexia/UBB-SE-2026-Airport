using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirportApp.Tests.IntegrationTests;

[TestFixture]
public class FAQControllerIntegrationTests
{
    private ApiTestBase _factory = null!;
    private HttpClient _client = null!;

    private const int NonExistentFaqId = 999999;

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
    public async Task GetById_WithNonExistentId_Returns404()
    {
        var response = await _client.GetAsync($"/api/faq/{NonExistentFaqId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
