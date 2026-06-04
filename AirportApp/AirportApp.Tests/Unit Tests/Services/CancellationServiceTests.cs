using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class CancellationServiceTests
{
    private const int DefaultTicketId = 1;
    private const int FutureFlightDaysOffset = 5;
    private const int PastFlightDaysOffset = -1;
    private const string ActiveStatus = "Active";
    private const string CancelledStatus = "Cancelled";
    private const string CancelledStatusLowercase = "cancelled";
    private const string TicketNotFoundMessage = "Ticket not found.";
    private const string AlreadyCancelledMessage = "already cancelled";
    private const string PastFlightMessage = "in the past";

    private IFlightTicketRepository _ticketRepository = null!;
    private CancellationService _cancellationService = null!;

    [SetUp]
    public void SetUp()
    {
        _ticketRepository = Substitute.For<IFlightTicketRepository>();
        _cancellationService = new CancellationService(_ticketRepository);
    }

    [Test]
    public void CanCancelTicket_NullTicket_ReturnsFalse()
    {
        var (canCancel, _) = _cancellationService.CanCancelTicket(null!);

        Assert.That(canCancel, Is.False);
    }

    [Test]
    public void CanCancelTicket_NullTicket_ReturnsTicketNotFoundReason()
    {
        var (_, reason) = _cancellationService.CanCancelTicket(null!);

        Assert.That(reason, Is.EqualTo(TicketNotFoundMessage));
    }

    [Test]
    public void CanCancelTicket_AlreadyCancelledTicket_ReturnsFalse()
    {
        var flight = new Flight { Date = DateTime.Now.AddDays(FutureFlightDaysOffset) };
        var ticket = new FlightTicket { Id = DefaultTicketId, Status = CancelledStatus, Flight = flight };

        var (canCancel, _) = _cancellationService.CanCancelTicket(ticket);

        Assert.That(canCancel, Is.False);
    }

    [Test]
    public void CanCancelTicket_AlreadyCancelledTicket_ReturnsAlreadyCancelledReason()
    {
        var flight = new Flight { Date = DateTime.Now.AddDays(FutureFlightDaysOffset) };
        var ticket = new FlightTicket { Id = DefaultTicketId, Status = CancelledStatus, Flight = flight };

        var (_, reason) = _cancellationService.CanCancelTicket(ticket);

        Assert.That(reason, Does.Contain(AlreadyCancelledMessage));
    }

    [Test]
    public void CanCancelTicket_CancelledStatusCaseInsensitive_ReturnsFalse()
    {
        var ticket = new FlightTicket { Id = DefaultTicketId, Status = CancelledStatusLowercase };

        var (canCancel, _) = _cancellationService.CanCancelTicket(ticket);

        Assert.That(canCancel, Is.False);
    }

    [Test]
    public void CanCancelTicket_PastFlightDate_ReturnsFalse()
    {
        var pastDate = DateTime.Now.AddDays(PastFlightDaysOffset);
        var flight = new Flight { Date = pastDate };
        var ticket = new FlightTicket { Id = DefaultTicketId, Status = ActiveStatus, Flight = flight };

        var (canCancel, _) = _cancellationService.CanCancelTicket(ticket);

        Assert.That(canCancel, Is.False);
    }

    [Test]
    public void CanCancelTicket_PastFlightDate_ReturnsPastFlightReason()
    {
        var pastDate = DateTime.Now.AddDays(PastFlightDaysOffset);
        var flight = new Flight { Date = pastDate };
        var ticket = new FlightTicket { Id = DefaultTicketId, Status = ActiveStatus, Flight = flight };

        var (_, reason) = _cancellationService.CanCancelTicket(ticket);

        Assert.That(reason, Does.Contain(PastFlightMessage));
    }

    [Test]
    public void CanCancelTicket_ActiveTicketWithFutureFlight_ReturnsTrue()
    {
        var futureDate = DateTime.Now.AddDays(FutureFlightDaysOffset);
        var flight = new Flight { Date = futureDate };
        var ticket = new FlightTicket { Id = DefaultTicketId, Status = ActiveStatus, Flight = flight };

        var (canCancel, _) = _cancellationService.CanCancelTicket(ticket);

        Assert.That(canCancel, Is.True);
    }

    [Test]
    public void CanCancelTicket_ActiveTicketWithFutureFlight_ReturnsEmptyReason()
    {
        var futureDate = DateTime.Now.AddDays(FutureFlightDaysOffset);
        var flight = new Flight { Date = futureDate };
        var ticket = new FlightTicket { Id = DefaultTicketId, Status = ActiveStatus, Flight = flight };

        var (_, reason) = _cancellationService.CanCancelTicket(ticket);

        Assert.That(reason, Is.Empty);
    }

    [Test]
    public void CanCancelTicket_ActiveTicketWithNullFlight_ReturnsTrue()
    {
        var ticket = new FlightTicket { Id = DefaultTicketId, Status = ActiveStatus, Flight = null! };

        var (canCancel, _) = _cancellationService.CanCancelTicket(ticket);

        Assert.That(canCancel, Is.True);
    }

    [Test]
    public void CanCancelTicket_ActiveTicketWithNullFlight_ReturnsEmptyReason()
    {
        var ticket = new FlightTicket { Id = DefaultTicketId, Status = ActiveStatus, Flight = null! };

        var (_, reason) = _cancellationService.CanCancelTicket(ticket);

        Assert.That(reason, Is.Empty);
    }

    [Test]
    public async Task CancelTicketAsync_ValidTicketId_CallsUpdateStatusWithCancelledStatus()
    {
        await _cancellationService.CancelTicketAsync(DefaultTicketId);

        await _ticketRepository.Received(1).UpdateStatusAsync(DefaultTicketId, CancelledStatus);
    }
}
