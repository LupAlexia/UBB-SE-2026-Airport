using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class ComplaintTicketSubcategoryServiceTests
{
    private const int ValidSubcategoryId = 1;
    private const int InvalidSubcategoryId = 99;
    private const string SubcategoryName = "Long delay";

    private static ComplaintTicketCategory CreateParentCategory() =>
        new ComplaintTicketCategory(1, "Delay", ComplaintTicketUrgencyLevelEnum.MEDIUM);

    [Test]
    public void GetSubcategoryByIdAsync_ThrowsKeyNotFoundException_WhenSubcategoryDoesNotExist()
    {
        var subcategoryRepository = Substitute.For<IComplaintTicketSubcategoryRepository>();
        subcategoryRepository.GetByIdAsync(InvalidSubcategoryId).Returns(Task.FromResult<ComplaintTicketSubcategory?>(null));
        var service = new ComplaintTicketSubcategoryService(subcategoryRepository);

        Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetSubcategoryByIdAsync(InvalidSubcategoryId));
    }

    [Test]
    public async Task GetSubcategoryByIdAsync_ReturnsSubcategory_WhenSubcategoryExists()
    {
        var subcategoryRepository = Substitute.For<IComplaintTicketSubcategoryRepository>();
        var subcategory = new ComplaintTicketSubcategory(ValidSubcategoryId, SubcategoryName, 0, CreateParentCategory());
        subcategoryRepository.GetByIdAsync(ValidSubcategoryId).Returns(Task.FromResult<ComplaintTicketSubcategory?>(subcategory));
        var service = new ComplaintTicketSubcategoryService(subcategoryRepository);

        var result = await service.GetSubcategoryByIdAsync(ValidSubcategoryId);

        Assert.That(result, Is.EqualTo(subcategory));
    }
}
