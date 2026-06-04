using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class FAQServiceTests
{
    private static FAQEntry CreateFAQEntry(int id, string question, string answer, FAQCategoryEnum category) =>
        new FAQEntry(id, question, answer, category, 0, 0, 0);

    [Test]
    public async Task FilterFAQEntryAsync_ReturnsAllEntries_WhenCategoryIsAll()
    {
        var faqRepository = Substitute.For<IFAQRepository>();
        var entries = new List<FAQEntry>
        {
            CreateFAQEntry(1, "Where is gate A1?", "It is in terminal 1.", FAQCategoryEnum.Facilities),
            CreateFAQEntry(2, "How do I check in?", "Use the kiosk.", FAQCategoryEnum.CheckIn),
        };
        faqRepository.GetAsync().Returns(Task.FromResult<IEnumerable<FAQEntry>>(entries));
        var service = new FAQService(faqRepository);

        var result = await service.FilterFAQEntryAsync(FAQCategoryEnum.All, null);

        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task FilterFAQEntryAsync_ReturnsOnlyMatchingCategory_WhenCategoryIsNotAll()
    {
        var faqRepository = Substitute.For<IFAQRepository>();
        var baggageEntries = new List<FAQEntry>
        {
            CreateFAQEntry(1, "What is the baggage limit?", "23 kg.", FAQCategoryEnum.Baggage),
        };
        faqRepository.GetByCategoryAsync(FAQCategoryEnum.Baggage).Returns(Task.FromResult<IEnumerable<FAQEntry>>(baggageEntries));
        var service = new FAQService(faqRepository);

        var result = await service.FilterFAQEntryAsync(FAQCategoryEnum.Baggage, null);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Category, Is.EqualTo(FAQCategoryEnum.Baggage));
    }

    [Test]
    public async Task FilterFAQEntryAsync_ReturnsAllEntries_WhenSearchQueryIsEmpty()
    {
        var faqRepository = Substitute.For<IFAQRepository>();
        var entries = new List<FAQEntry>
        {
            CreateFAQEntry(1, "How do I check in?", "Use the kiosk.", FAQCategoryEnum.CheckIn),
            CreateFAQEntry(2, "Where is parking?", "Level B.", FAQCategoryEnum.Parking),
        };
        faqRepository.GetAsync().Returns(Task.FromResult<IEnumerable<FAQEntry>>(entries));
        var service = new FAQService(faqRepository);

        var result = await service.FilterFAQEntryAsync(FAQCategoryEnum.All, string.Empty);

        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task FilterFAQEntryAsync_FiltersEntries_WhenSearchQueryMatchesQuestion()
    {
        var faqRepository = Substitute.For<IFAQRepository>();
        var entries = new List<FAQEntry>
        {
            CreateFAQEntry(1, "What is the baggage limit?", "23 kg.", FAQCategoryEnum.Baggage),
            CreateFAQEntry(2, "How do I check in?", "Use the kiosk.", FAQCategoryEnum.CheckIn),
        };
        faqRepository.GetAsync().Returns(Task.FromResult<IEnumerable<FAQEntry>>(entries));
        var service = new FAQService(faqRepository);

        var result = await service.FilterFAQEntryAsync(FAQCategoryEnum.All, "baggage");

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(1));
    }

    [Test]
    public async Task FilterFAQEntryAsync_FiltersEntries_WhenSearchQueryMatchesAnswer()
    {
        var faqRepository = Substitute.For<IFAQRepository>();
        var entries = new List<FAQEntry>
        {
            CreateFAQEntry(1, "How do I check in?", "Use the kiosk.", FAQCategoryEnum.CheckIn),
            CreateFAQEntry(2, "Where is parking?", "Level B.", FAQCategoryEnum.Parking),
        };
        faqRepository.GetAsync().Returns(Task.FromResult<IEnumerable<FAQEntry>>(entries));
        var service = new FAQService(faqRepository);

        var result = await service.FilterFAQEntryAsync(FAQCategoryEnum.All, "kiosk");

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(1));
    }

    [Test]
    public async Task FilterFAQEntryAsync_ReturnsEmptyList_WhenSearchQueryMatchesNothing()
    {
        var faqRepository = Substitute.For<IFAQRepository>();
        var entries = new List<FAQEntry>
        {
            CreateFAQEntry(1, "How do I check in?", "Use the kiosk.", FAQCategoryEnum.CheckIn),
            CreateFAQEntry(2, "Where is parking?", "Level B.", FAQCategoryEnum.Parking),
        };
        faqRepository.GetAsync().Returns(Task.FromResult<IEnumerable<FAQEntry>>(entries));
        var service = new FAQService(faqRepository);

        var result = await service.FilterFAQEntryAsync(FAQCategoryEnum.All, "xyznotfound");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task FilterFAQEntryAsync_SearchIsCaseInsensitive_WhenQueryDiffersByCase()
    {
        var faqRepository = Substitute.For<IFAQRepository>();
        var entries = new List<FAQEntry>
        {
            CreateFAQEntry(1, "What is the Baggage limit?", "23 kg.", FAQCategoryEnum.Baggage),
        };
        faqRepository.GetAsync().Returns(Task.FromResult<IEnumerable<FAQEntry>>(entries));
        var service = new FAQService(faqRepository);

        var result = await service.FilterFAQEntryAsync(FAQCategoryEnum.All, "BAGGAGE");

        Assert.That(result.Count, Is.EqualTo(1));
    }
}
