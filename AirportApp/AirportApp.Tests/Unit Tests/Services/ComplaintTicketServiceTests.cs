using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class ComplaintTicketServiceTests
{
    private const int ValidTicketId = 1;
    private const int InvalidTicketId = 99;
    private const int ValidCategoryId = 10;
    private const int DifferentCategoryId = 20;
    private const string ValidSubject = "Flight was delayed";
    private const string ValidDescription = "I waited for 3 hours";

    private static User CreateUser() => new User(1, "John Doe", "john@example.com");

    private static ComplaintTicketCategory CreateCategory(int id = ValidCategoryId) =>
        new ComplaintTicketCategory(id, "Delay", ComplaintTicketUrgencyLevelEnum.MEDIUM);

    private static ComplaintTicketSubcategory CreateSubcategory(int parentCategoryId = ValidCategoryId) =>
        new ComplaintTicketSubcategory(1, "Long delay", 0, CreateCategory(parentCategoryId));

    [Test]
    public void CreateTicketAsync_ThrowsArgumentNullException_WhenCreatorIsNull()
    {
        var ticketRepository = Substitute.For<IComplaintTicketRepository>();
        var service = new ComplaintTicketService(ticketRepository);

        Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.CreateTicketAsync(ValidTicketId, null!, ComplaintTicketStatusEnum.OPEN,
                CreateCategory(), CreateSubcategory(), ValidSubject, ValidDescription, DateTime.Now));
    }

    [Test]
    public void CreateTicketAsync_ThrowsArgumentNullException_WhenCategoryIsNull()
    {
        var ticketRepository = Substitute.For<IComplaintTicketRepository>();
        var service = new ComplaintTicketService(ticketRepository);

        Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.CreateTicketAsync(ValidTicketId, CreateUser(), ComplaintTicketStatusEnum.OPEN,
                null!, CreateSubcategory(), ValidSubject, ValidDescription, DateTime.Now));
    }

    [Test]
    public void CreateTicketAsync_ThrowsArgumentNullException_WhenSubcategoryIsNull()
    {
        var ticketRepository = Substitute.For<IComplaintTicketRepository>();
        var service = new ComplaintTicketService(ticketRepository);

        Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.CreateTicketAsync(ValidTicketId, CreateUser(), ComplaintTicketStatusEnum.OPEN,
                CreateCategory(), null!, ValidSubject, ValidDescription, DateTime.Now));
    }

    [Test]
    public void CreateTicketAsync_ThrowsArgumentNullException_WhenSubjectIsEmpty()
    {
        var ticketRepository = Substitute.For<IComplaintTicketRepository>();
        var service = new ComplaintTicketService(ticketRepository);

        Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.CreateTicketAsync(ValidTicketId, CreateUser(), ComplaintTicketStatusEnum.OPEN,
                CreateCategory(), CreateSubcategory(), string.Empty, ValidDescription, DateTime.Now));
    }

    [Test]
    public void CreateTicketAsync_ThrowsArgumentNullException_WhenDescriptionIsWhitespace()
    {
        var ticketRepository = Substitute.For<IComplaintTicketRepository>();
        var service = new ComplaintTicketService(ticketRepository);

        Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.CreateTicketAsync(ValidTicketId, CreateUser(), ComplaintTicketStatusEnum.OPEN,
                CreateCategory(), CreateSubcategory(), ValidSubject, " ", DateTime.Now));
    }

    [Test]
    public void CreateTicketAsync_ThrowsArgumentException_WhenSubcategoryDoesNotBelongToCategory()
    {
        var ticketRepository = Substitute.For<IComplaintTicketRepository>();
        var service = new ComplaintTicketService(ticketRepository);
        var category = CreateCategory(ValidCategoryId);
        var subcategoryWithWrongParent = CreateSubcategory(DifferentCategoryId);

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateTicketAsync(ValidTicketId, CreateUser(), ComplaintTicketStatusEnum.OPEN,
                category, subcategoryWithWrongParent, ValidSubject, ValidDescription, DateTime.Now));
    }

    [Test]
    public async Task CreateTicketAsync_CallsRepositoryAdd_WhenDataIsValid()
    {
        var ticketRepository = Substitute.For<IComplaintTicketRepository>();
        var service = new ComplaintTicketService(ticketRepository);

        await service.CreateTicketAsync(ValidTicketId, CreateUser(), ComplaintTicketStatusEnum.OPEN,
            CreateCategory(), CreateSubcategory(), ValidSubject, ValidDescription, DateTime.Now);

        await ticketRepository.Received(1).AddAsync(Arg.Any<ComplaintTicket>());
    }

    [Test]
    public void GetTicketByIdAsync_ThrowsKeyNotFoundException_WhenTicketDoesNotExist()
    {
        var ticketRepository = Substitute.For<IComplaintTicketRepository>();
        ticketRepository.GetByIdAsync(InvalidTicketId).Returns(Task.FromResult<ComplaintTicket?>(null));
        var service = new ComplaintTicketService(ticketRepository);

        Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetTicketByIdAsync(InvalidTicketId));
    }

    [Test]
    public async Task GetTicketByIdAsync_ReturnsTicket_WhenTicketExists()
    {
        var ticketRepository = Substitute.For<IComplaintTicketRepository>();
        var ticket = new ComplaintTicket { Id = ValidTicketId, Subject = ValidSubject };
        ticketRepository.GetByIdAsync(ValidTicketId).Returns(Task.FromResult<ComplaintTicket?>(ticket));
        var service = new ComplaintTicketService(ticketRepository);

        var result = await service.GetTicketByIdAsync(ValidTicketId);

        Assert.That(result, Is.EqualTo(ticket));
    }

    [Test]
    public async Task FilterTicketsByStatusAsync_ReturnsOnlyOpenTickets_WhenFilterIsOpen()
    {
        var ticketRepository = Substitute.For<IComplaintTicketRepository>();
        var service = new ComplaintTicketService(ticketRepository);
        var tickets = new List<TicketDTO>
        {
            new TicketDTO(1, 1, "a@a.com", ComplaintTicketUrgencyLevelEnum.LOW, ComplaintTicketStatusEnum.OPEN, 1, "Cat", 1, "Sub", "Subject1", "Desc1", DateTime.Now),
            new TicketDTO(2, 2, "b@b.com", ComplaintTicketUrgencyLevelEnum.MEDIUM, ComplaintTicketStatusEnum.IN_PROGRESS, 1, "Cat", 1, "Sub", "Subject2", "Desc2", DateTime.Now),
            new TicketDTO(3, 3, "c@c.com", ComplaintTicketUrgencyLevelEnum.HIGH, ComplaintTicketStatusEnum.RESOLVED, 1, "Cat", 1, "Sub", "Subject3", "Desc3", DateTime.Now),
        };

        var result = (await service.FilterTicketsByStatusAsync(tickets, TicketFilterStatusEnum.OPEN)).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].currentStatus, Is.EqualTo(ComplaintTicketStatusEnum.OPEN));
    }

    [Test]
    public async Task FilterTicketsByStatusAsync_ReturnsOnlyInProgressTickets_WhenFilterIsInProgress()
    {
        var ticketRepository = Substitute.For<IComplaintTicketRepository>();
        var service = new ComplaintTicketService(ticketRepository);
        var tickets = new List<TicketDTO>
        {
            new TicketDTO(1, 1, "a@a.com", ComplaintTicketUrgencyLevelEnum.LOW, ComplaintTicketStatusEnum.OPEN, 1, "Cat", 1, "Sub", "Subject1", "Desc1", DateTime.Now),
            new TicketDTO(2, 2, "b@b.com", ComplaintTicketUrgencyLevelEnum.MEDIUM, ComplaintTicketStatusEnum.IN_PROGRESS, 1, "Cat", 1, "Sub", "Subject2", "Desc2", DateTime.Now),
        };

        var result = (await service.FilterTicketsByStatusAsync(tickets, TicketFilterStatusEnum.IN_PROGRESS)).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].currentStatus, Is.EqualTo(ComplaintTicketStatusEnum.IN_PROGRESS));
    }

    [Test]
    public async Task FilterTicketsByStatusAsync_ReturnsOnlyResolvedTickets_WhenFilterIsResolved()
    {
        var ticketRepository = Substitute.For<IComplaintTicketRepository>();
        var service = new ComplaintTicketService(ticketRepository);
        var tickets = new List<TicketDTO>
        {
            new TicketDTO(1, 1, "a@a.com", ComplaintTicketUrgencyLevelEnum.LOW, ComplaintTicketStatusEnum.IN_PROGRESS, 1, "Cat", 1, "Sub", "Subject1", "Desc1", DateTime.Now),
            new TicketDTO(2, 2, "b@b.com", ComplaintTicketUrgencyLevelEnum.MEDIUM, ComplaintTicketStatusEnum.RESOLVED, 1, "Cat", 1, "Sub", "Subject2", "Desc2", DateTime.Now),
        };

        var result = (await service.FilterTicketsByStatusAsync(tickets, TicketFilterStatusEnum.RESOLVED)).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].currentStatus, Is.EqualTo(ComplaintTicketStatusEnum.RESOLVED));
    }

    [Test]
    public async Task FilterTicketsByStatusAsync_ReturnsAllTickets_WhenFilterIsAll()
    {
        var ticketRepository = Substitute.For<IComplaintTicketRepository>();
        var service = new ComplaintTicketService(ticketRepository);
        var tickets = new List<TicketDTO>
        {
            new TicketDTO(1, 1, "a@a.com", ComplaintTicketUrgencyLevelEnum.LOW, ComplaintTicketStatusEnum.OPEN, 1, "Cat", 1, "Sub", "Subject1", "Desc1", DateTime.Now),
            new TicketDTO(2, 2, "b@b.com", ComplaintTicketUrgencyLevelEnum.MEDIUM, ComplaintTicketStatusEnum.IN_PROGRESS, 1, "Cat", 1, "Sub", "Subject2", "Desc2", DateTime.Now),
            new TicketDTO(3, 3, "c@c.com", ComplaintTicketUrgencyLevelEnum.HIGH, ComplaintTicketStatusEnum.RESOLVED, 1, "Cat", 1, "Sub", "Subject3", "Desc3", DateTime.Now),
        };

        var result = (await service.FilterTicketsByStatusAsync(tickets, TicketFilterStatusEnum.ALL)).ToList();

        Assert.That(result.Count, Is.EqualTo(3));
    }
}
