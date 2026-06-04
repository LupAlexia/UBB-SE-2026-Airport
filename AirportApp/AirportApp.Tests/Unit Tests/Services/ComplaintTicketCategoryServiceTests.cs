using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class ComplaintTicketCategoryServiceTests
{
    private const int ValidCategoryId = 1;
    private const int InvalidCategoryId = 99;
    private const string CategoryName = "Delay";

    [Test]
    public void GetCategoryByIdAsync_CategoryDoesNotExist_ThrowsKeyNotFoundException()
    {
        var categoryRepository = Substitute.For<IComplaintTicketCategoryRepository>();
        categoryRepository.GetByIdAsync(InvalidCategoryId).Returns(Task.FromResult<ComplaintTicketCategory?>(null));
        var service = new ComplaintTicketCategoryService(categoryRepository);

        Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetCategoryByIdAsync(InvalidCategoryId));
    }

    [Test]
    public async Task GetCategoryByIdAsync_CategoryExists_ReturnsCategory()
    {
        var categoryRepository = Substitute.For<IComplaintTicketCategoryRepository>();
        var category = new ComplaintTicketCategory(ValidCategoryId, CategoryName, ComplaintTicketUrgencyLevelEnum.MEDIUM);
        categoryRepository.GetByIdAsync(ValidCategoryId).Returns(Task.FromResult<ComplaintTicketCategory?>(category));
        var service = new ComplaintTicketCategoryService(categoryRepository);

        var result = await service.GetCategoryByIdAsync(ValidCategoryId);

        Assert.That(result, Is.EqualTo(category));
    }
}
