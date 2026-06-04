using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class DashboardServiceTests
{
    private const int ValidUserId = 1;
    private static readonly DateTime PastDate1 = new DateTime(2020, 1, 1);
    private static readonly DateTime PastDate2 = new DateTime(2021, 6, 15);
    private static readonly DateTime FutureDate1 = new DateTime(2030, 1, 1);
    private static readonly DateTime FutureDate2 = new DateTime(2030, 6, 15);

    private static FlightTicket CreateTicketWithFlight(DateTime flightDate) =>
        new FlightTicket { Flight = new Flight { Date = flightDate } };

    private static FlightTicket CreateTicketWithNoFlight() =>
        new FlightTicket { Flight = null! };

    [Test]
    public async Task GetUserTicketsAsync_SomeTicketsHaveNoFlight_ExcludesTicketsWithNullFlight()
    {
        var ticketRepository = Substitute.For<IFlightTicketRepository>();
        var tickets = new List<FlightTicket>
        {
            CreateTicketWithFlight(FutureDate1),
            CreateTicketWithNoFlight(),
        };
        ticketRepository.GetByUserIdAsync(ValidUserId).Returns(Task.FromResult<IEnumerable<FlightTicket>>(tickets));
        var service = new DashboardService(ticketRepository);

        var result = (await service.GetUserTicketsAsync(ValidUserId, "Upcoming")).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Flight, Is.Not.Null);
    }

    [Test]
    public async Task GetUserTicketsAsync_FilterIsPast_ReturnsOnlyPastFlights()
    {
        var ticketRepository = Substitute.For<IFlightTicketRepository>();
        var tickets = new List<FlightTicket>
        {
            CreateTicketWithFlight(PastDate1),
            CreateTicketWithFlight(FutureDate1),
        };
        ticketRepository.GetByUserIdAsync(ValidUserId).Returns(Task.FromResult<IEnumerable<FlightTicket>>(tickets));
        var service = new DashboardService(ticketRepository);

        var result = (await service.GetUserTicketsAsync(ValidUserId, "Past")).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Flight.Date, Is.EqualTo(PastDate1));
    }

    [Test]
    public async Task GetUserTicketsAsync_FilterIsNotPast_ReturnsOnlyUpcomingFlights()
    {
        var ticketRepository = Substitute.For<IFlightTicketRepository>();
        var tickets = new List<FlightTicket>
        {
            CreateTicketWithFlight(PastDate1),
            CreateTicketWithFlight(FutureDate1),
        };
        ticketRepository.GetByUserIdAsync(ValidUserId).Returns(Task.FromResult<IEnumerable<FlightTicket>>(tickets));
        var service = new DashboardService(ticketRepository);

        var result = (await service.GetUserTicketsAsync(ValidUserId, "Upcoming")).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Flight.Date, Is.EqualTo(FutureDate1));
    }

    [Test]
    public async Task GetUserTicketsAsync_FilterIsPastWithMultipleFlights_ReturnsPastFlightsOrderedDescending()
    {
        var ticketRepository = Substitute.For<IFlightTicketRepository>();
        var tickets = new List<FlightTicket>
        {
            CreateTicketWithFlight(PastDate1),
            CreateTicketWithFlight(PastDate2),
        };
        ticketRepository.GetByUserIdAsync(ValidUserId).Returns(Task.FromResult<IEnumerable<FlightTicket>>(tickets));
        var service = new DashboardService(ticketRepository);

        var result = (await service.GetUserTicketsAsync(ValidUserId, "Past")).ToList();

        Assert.That(result[0].Flight.Date, Is.EqualTo(PastDate2));
        Assert.That(result[1].Flight.Date, Is.EqualTo(PastDate1));
    }

    [Test]
    public async Task GetUserTicketsAsync_FilterIsNotPastWithMultipleFlights_ReturnsUpcomingFlightsOrderedAscending()
    {
        var ticketRepository = Substitute.For<IFlightTicketRepository>();
        var tickets = new List<FlightTicket>
        {
            CreateTicketWithFlight(FutureDate2),
            CreateTicketWithFlight(FutureDate1),
        };
        ticketRepository.GetByUserIdAsync(ValidUserId).Returns(Task.FromResult<IEnumerable<FlightTicket>>(tickets));
        var service = new DashboardService(ticketRepository);

        var result = (await service.GetUserTicketsAsync(ValidUserId, "Upcoming")).ToList();

        Assert.That(result[0].Flight.Date, Is.EqualTo(FutureDate1));
        Assert.That(result[1].Flight.Date, Is.EqualTo(FutureDate2));
    }

    [Test]
    public async Task GetUserTicketsAsync_FilterCaseVaries_IsCaseInsensitive()
    {
        var ticketRepository = Substitute.For<IFlightTicketRepository>();
        var tickets = new List<FlightTicket>
        {
            CreateTicketWithFlight(PastDate1),
            CreateTicketWithFlight(FutureDate1),
        };
        ticketRepository.GetByUserIdAsync(ValidUserId).Returns(Task.FromResult<IEnumerable<FlightTicket>>(tickets));
        var service = new DashboardService(ticketRepository);

        var result = (await service.GetUserTicketsAsync(ValidUserId, "past")).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Flight.Date, Is.EqualTo(PastDate1));
    }
}
