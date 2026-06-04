using AirportApp.ClassLibrary.Entity.Dto;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AirportApp.Tests.IntegrationTests;

[TestFixture]
public class BookingControllerIntegrationTests
{
    private ApiTestBase _factory = null!;
    private HttpClient _client = null!;

    private const int StandardRouteCapacity = 200;
    private const int StandardOccupiedSeatCount = 50;
    private const int StandardRequestedPassengerCount = 3;
    private const int StandardSeatMapCapacity = 180;
    private const int StandardMaxPassengers = 10;

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
    public async Task ValidatePassengers_WithValidPassengerData_ReturnsOkStatusCode()
    {
        var passengers = new List<PassengerDataDTO>
        {
            new PassengerDataDTO
            {
                FirstName = "Ion",
                LastName = "Popescu",
                Email = "ion.popescu@example.com",
                Phone = "0740000000",
                SelectedSeat = "1A",
                SelectedAddOns = new List<PricingAddOnDTO>()
            }
        };

        var response = await _client.PostAsJsonAsync("/api/booking/validate-passengers", passengers);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task CalculateMaxPassengers_WithValidQueryParameters_ReturnsOkStatusCode()
    {
        var response = await _client.GetAsync(
            $"/api/booking/calculate-max-passengers" +
            $"?routeCapacity={StandardRouteCapacity}" +
            $"&occupiedSeatCount={StandardOccupiedSeatCount}" +
            $"&requestedPassengerCount={StandardRequestedPassengerCount}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task BuildSeatMap_WithValidCapacity_ReturnsOkStatusCode()
    {
        var response = await _client.GetAsync(
            $"/api/booking/build-seat-map?capacity={StandardSeatMapCapacity}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetInitialPassengerCount_WithValidQueryParameters_ReturnsOkStatusCode()
    {
        var response = await _client.GetAsync(
            $"/api/booking/initial-passenger-count" +
            $"?maxPassengers={StandardMaxPassengers}" +
            $"&requestedCount={StandardRequestedPassengerCount}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
