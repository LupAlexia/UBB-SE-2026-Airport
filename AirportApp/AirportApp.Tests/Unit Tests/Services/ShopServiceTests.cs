using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class ShopServiceTests
{
    private const int DefaultShopId = 1;
    private const int SecondShopId = 2;
    private const string DefaultShopName = "Coffee Corner";
    private const string DefaultShopType = "Cafe";
    private const string AlternativeShopName = "Book Store";
    private const string WhitespaceString = "   ";
    private const string SearchTextWithNoMatch = "XYZ";
    private const string EmptyNameErrorMessageFragment = "shop name field must not be empty";
    private const string EmptyTypeErrorMessageFragment = "shop type field must not be empty";
    private const string DuplicateNameErrorMessageFragment = "A shop with this name already exists";

    private IShopRepository _shopRepository = null!;
    private ShopService _shopService = null!;

    [SetUp]
    public void SetUp()
    {
        _shopRepository = Substitute.For<IShopRepository>();
        _shopService = new ShopService(_shopRepository);
    }

    private static Shop CreateShop(int shopId, string name, string type = DefaultShopType) =>
        new Shop(shopId, name, type, null!);

    [Test]
    public void AddShopAsync_NullName_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var shop = new Shop(DefaultShopId, null!, DefaultShopType, null!);

        var exception = Assert.ThrowsAsync<ArgumentException>(() => _shopService.AddShopAsync(shop));

        Assert.That(exception!.Message, Does.Contain(EmptyNameErrorMessageFragment));
    }

    [Test]
    public void AddShopAsync_EmptyName_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var shop = CreateShop(DefaultShopId, string.Empty, DefaultShopType);

        var exception = Assert.ThrowsAsync<ArgumentException>(() => _shopService.AddShopAsync(shop));

        Assert.That(exception!.Message, Does.Contain(EmptyNameErrorMessageFragment));
    }

    [Test]
    public void AddShopAsync_WhitespaceName_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var shop = CreateShop(DefaultShopId, WhitespaceString, DefaultShopType);

        var exception = Assert.ThrowsAsync<ArgumentException>(() => _shopService.AddShopAsync(shop));

        Assert.That(exception!.Message, Does.Contain(EmptyNameErrorMessageFragment));
    }

    [Test]
    public void AddShopAsync_NullType_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var shop = new Shop(DefaultShopId, DefaultShopName, null!, null!);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop>()));

        var exception = Assert.ThrowsAsync<ArgumentException>(() => _shopService.AddShopAsync(shop));

        Assert.That(exception!.Message, Does.Contain(EmptyTypeErrorMessageFragment));
    }

    [Test]
    public void AddShopAsync_EmptyType_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var shop = CreateShop(DefaultShopId, DefaultShopName, string.Empty);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop>()));

        var exception = Assert.ThrowsAsync<ArgumentException>(() => _shopService.AddShopAsync(shop));

        Assert.That(exception!.Message, Does.Contain(EmptyTypeErrorMessageFragment));
    }

    [Test]
    public void AddShopAsync_WhitespaceType_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var shop = CreateShop(DefaultShopId, DefaultShopName, WhitespaceString);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop>()));

        var exception = Assert.ThrowsAsync<ArgumentException>(() => _shopService.AddShopAsync(shop));

        Assert.That(exception!.Message, Does.Contain(EmptyTypeErrorMessageFragment));
    }

    [Test]
    public void AddShopAsync_DuplicateName_ThrowsInvalidOperationExceptionWithCorrectMessage()
    {
        var existingShop = CreateShop(SecondShopId, DefaultShopName);
        var shopToAdd = CreateShop(DefaultShopId, DefaultShopName);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop> { existingShop }));

        var exception = Assert.ThrowsAsync<InvalidOperationException>(() => _shopService.AddShopAsync(shopToAdd));

        Assert.That(exception!.Message, Does.Contain(DuplicateNameErrorMessageFragment));
    }

    [Test]
    public void AddShopAsync_DuplicateNameCaseInsensitive_ThrowsInvalidOperationException()
    {
        var existingShop = CreateShop(SecondShopId, DefaultShopName.ToUpper());
        var shopToAdd = CreateShop(DefaultShopId, DefaultShopName.ToLower());
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop> { existingShop }));

        Assert.ThrowsAsync<InvalidOperationException>(() => _shopService.AddShopAsync(shopToAdd));
    }

    [Test]
    public async Task AddShopAsync_ValidShopWithUniqueName_CallsRepositoryAdd()
    {
        var shopToAdd = CreateShop(DefaultShopId, DefaultShopName);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop>()));

        await _shopService.AddShopAsync(shopToAdd);

        await _shopRepository.Received(1).AddAsync(shopToAdd);
    }

    [Test]
    public async Task AddShopAsync_DuplicateName_DoesNotCallRepositoryAdd()
    {
        var existingShop = CreateShop(SecondShopId, DefaultShopName);
        var shopToAdd = CreateShop(DefaultShopId, DefaultShopName);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop> { existingShop }));

        try { await _shopService.AddShopAsync(shopToAdd); } catch (InvalidOperationException) { }

        await _shopRepository.DidNotReceive().AddAsync(Arg.Any<Shop>());
    }

    [Test]
    public void UpdateShopAsync_NullName_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var shop = new Shop(DefaultShopId, null!, DefaultShopType, null!);

        var exception = Assert.ThrowsAsync<ArgumentException>(() => _shopService.UpdateShopAsync(shop));

        Assert.That(exception!.Message, Does.Contain(EmptyNameErrorMessageFragment));
    }

    [Test]
    public void UpdateShopAsync_EmptyName_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var shop = CreateShop(DefaultShopId, string.Empty, DefaultShopType);

        var exception = Assert.ThrowsAsync<ArgumentException>(() => _shopService.UpdateShopAsync(shop));

        Assert.That(exception!.Message, Does.Contain(EmptyNameErrorMessageFragment));
    }

    [Test]
    public void UpdateShopAsync_WhitespaceName_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var shop = CreateShop(DefaultShopId, WhitespaceString, DefaultShopType);

        var exception = Assert.ThrowsAsync<ArgumentException>(() => _shopService.UpdateShopAsync(shop));

        Assert.That(exception!.Message, Does.Contain(EmptyNameErrorMessageFragment));
    }

    [Test]
    public void UpdateShopAsync_NullType_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var shop = new Shop(DefaultShopId, DefaultShopName, null!, null!);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop>()));

        var exception = Assert.ThrowsAsync<ArgumentException>(() => _shopService.UpdateShopAsync(shop));

        Assert.That(exception!.Message, Does.Contain(EmptyTypeErrorMessageFragment));
    }

    [Test]
    public void UpdateShopAsync_EmptyType_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var shop = CreateShop(DefaultShopId, DefaultShopName, string.Empty);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop>()));

        var exception = Assert.ThrowsAsync<ArgumentException>(() => _shopService.UpdateShopAsync(shop));

        Assert.That(exception!.Message, Does.Contain(EmptyTypeErrorMessageFragment));
    }

    [Test]
    public void UpdateShopAsync_WhitespaceType_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var shop = CreateShop(DefaultShopId, DefaultShopName, WhitespaceString);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop>()));

        var exception = Assert.ThrowsAsync<ArgumentException>(() => _shopService.UpdateShopAsync(shop));

        Assert.That(exception!.Message, Does.Contain(EmptyTypeErrorMessageFragment));
    }

    [Test]
    public void UpdateShopAsync_DuplicateNameBelongingToOtherShop_ThrowsInvalidOperationExceptionWithCorrectMessage()
    {
        var otherShop = CreateShop(SecondShopId, DefaultShopName);
        var shopToUpdate = CreateShop(DefaultShopId, DefaultShopName);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop> { otherShop }));

        var exception = Assert.ThrowsAsync<InvalidOperationException>(() => _shopService.UpdateShopAsync(shopToUpdate));

        Assert.That(exception!.Message, Does.Contain(DuplicateNameErrorMessageFragment));
    }

    [Test]
    public async Task UpdateShopAsync_SameNameBelongingToSameShop_CallsRepositoryUpdate()
    {
        var shopToUpdate = CreateShop(DefaultShopId, DefaultShopName);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop> { shopToUpdate }));

        await _shopService.UpdateShopAsync(shopToUpdate);

        await _shopRepository.Received(1).UpdateAsync(shopToUpdate);
    }

    [Test]
    public async Task UpdateShopAsync_ValidShopWithUniqueName_CallsRepositoryUpdate()
    {
        var shopToUpdate = CreateShop(DefaultShopId, DefaultShopName);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop>()));

        await _shopService.UpdateShopAsync(shopToUpdate);

        await _shopRepository.Received(1).UpdateAsync(shopToUpdate);
    }

    [Test]
    public async Task UpdateShopAsync_DuplicateName_DoesNotCallRepositoryUpdate()
    {
        var otherShop = CreateShop(SecondShopId, DefaultShopName);
        var shopToUpdate = CreateShop(DefaultShopId, DefaultShopName);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop> { otherShop }));

        try { await _shopService.UpdateShopAsync(shopToUpdate); } catch (InvalidOperationException) { }

        await _shopRepository.DidNotReceive().UpdateAsync(Arg.Any<Shop>());
    }

    [Test]
    public async Task DeleteShopAsync_ValidShopId_CallsRepositoryDelete()
    {
        await _shopService.DeleteShopAsync(DefaultShopId);

        await _shopRepository.Received(1).DeleteAsync(DefaultShopId);
    }

    [Test]
    public async Task SortAlphabeticallyAsync_MultipleShops_ReturnsSortedByNameAscending()
    {
        var shops = new List<Shop>
        {
            CreateShop(1, "Zara"),
            CreateShop(2, "Apple Store"),
            CreateShop(3, "Coffee Corner")
        };
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(shops));

        var result = (await _shopService.SortAlphabeticallyAsync()).ToList();

        Assert.That(result.Select(s => s.Name), Is.EqualTo(new[] { "Apple Store", "Coffee Corner", "Zara" }));
    }

    [Test]
    public async Task SortAlphabeticallyAsync_EmptyShopList_ReturnsEmptyCollection()
    {
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop>()));

        var result = await _shopService.SortAlphabeticallyAsync();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task SortAlphabeticallyAsync_SortingIsCaseInsensitive_ReturnsSortedCorrectly()
    {
        var shops = new List<Shop>
        {
            CreateShop(1, "zara"),
            CreateShop(2, "Apple Store"),
            CreateShop(3, "coffee")
        };
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(shops));

        var result = (await _shopService.SortAlphabeticallyAsync()).ToList();

        Assert.That(result.Select(s => s.Name), Is.EqualTo(new[] { "Apple Store", "coffee", "zara" }));
    }

    [Test]
    public async Task SearchByNameAsync_ExactMatch_ReturnsMatchingShop()
    {
        var matchingShop = CreateShop(DefaultShopId, DefaultShopName);
        var otherShop = CreateShop(SecondShopId, AlternativeShopName);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop> { matchingShop, otherShop }));

        var result = await _shopService.SearchByNameAsync(DefaultShopName);

        Assert.That(result, Is.EquivalentTo(new[] { matchingShop }));
    }

    [Test]
    public async Task SearchByNameAsync_PartialMatch_ReturnsShopsContainingText()
    {
        var matchingShop = CreateShop(DefaultShopId, DefaultShopName);
        var otherShop = CreateShop(SecondShopId, AlternativeShopName);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop> { matchingShop, otherShop }));

        var result = await _shopService.SearchByNameAsync("Coffee");

        Assert.That(result, Is.EquivalentTo(new[] { matchingShop }));
    }

    [Test]
    public async Task SearchByNameAsync_NoMatch_ReturnsEmptyCollection()
    {
        var shop = CreateShop(DefaultShopId, DefaultShopName);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop> { shop }));

        var result = await _shopService.SearchByNameAsync(SearchTextWithNoMatch);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task SearchByNameAsync_SearchIsCaseInsensitive_ReturnsMatchingShops()
    {
        var matchingShop = CreateShop(DefaultShopId, DefaultShopName);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop> { matchingShop }));

        var result = await _shopService.SearchByNameAsync(DefaultShopName.ToUpper());

        Assert.That(result, Is.EquivalentTo(new[] { matchingShop }));
    }

    [Test]
    public async Task SearchByNameAsync_MultipleMatches_ReturnsAllMatchingShops()
    {
        var firstMatchingShop = CreateShop(DefaultShopId, "Coffee Corner");
        var secondMatchingShop = CreateShop(SecondShopId, "Coffee Express");
        var nonMatchingShop = CreateShop(3, AlternativeShopName);
        _shopRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Shop>>(new List<Shop> { firstMatchingShop, secondMatchingShop, nonMatchingShop }));

        var result = await _shopService.SearchByNameAsync("Coffee");

        Assert.That(result, Is.EquivalentTo(new[] { firstMatchingShop, secondMatchingShop }));
    }
}
