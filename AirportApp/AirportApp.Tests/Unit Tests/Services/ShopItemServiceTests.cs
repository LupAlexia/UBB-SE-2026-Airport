using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class ShopItemServiceTests
{
    private const int DefaultShopItemId = 1;
    private const int DefaultShopId = 1;
    private const int SecondShopId = 2;
    private const int DefaultQuantity = 10;
    private const int NegativeQuantity = -1;
    private const int ZeroShopId = 0;
    private const float DefaultPrice = 5.0f;
    private const float ZeroPrice = 0.0f;
    private const float NegativePrice = -1.0f;
    private const float CheapItemPrice = 1.0f;
    private const float ExpensiveItemPrice = 10.0f;
    private const string DefaultItemName = "Test Item";
    private const string WhitespaceString = "   ";

    private static readonly Manager TestManager = new Manager(1, "Test Manager", "manager@test.com", "0700000000");
    private static readonly Shop TestShop = new Shop(1, "Test Shop", "Duty Free", TestManager);
    private static readonly Shop SecondShop = new Shop(2, "Second Shop", "Duty Free", TestManager);
    private static readonly Shop InvalidShop = new Shop(ZeroShopId, "Invalid Shop", "Duty Free", TestManager);

    private IShopItemRepository _shopItemRepository = null!;
    private ShopItemService _shopItemService = null!;

    [SetUp]
    public void SetUp()
    {
        _shopItemRepository = Substitute.For<IShopItemRepository>();
        _shopItemService = new ShopItemService(_shopItemRepository);
    }

    private static ShopItem CreateShopItem(int id, Shop shop, string name = DefaultItemName,
        int quantity = DefaultQuantity, float price = DefaultPrice) =>
        new ShopItem(id, quantity, price, shop, string.Empty, name, string.Empty);

    // --- GetByIdAsync ---

    [Test]
    public void GetByIdAsync_ItemNotFound_ThrowsInvalidOperationException()
    {
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(null));

        Assert.ThrowsAsync<InvalidOperationException>(() => _shopItemService.GetByIdAsync(DefaultShopItemId));
    }

    [Test]
    public async Task GetByIdAsync_ItemExists_ReturnsItem()
    {
        var existingItem = CreateShopItem(DefaultShopItemId, TestShop);
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(existingItem));

        var result = await _shopItemService.GetByIdAsync(DefaultShopItemId);

        Assert.That(result, Is.EqualTo(existingItem));
    }

    // --- GetItemsByShopIdAsync ---

    [Test]
    public async Task GetItemsByShopIdAsync_ItemsExistForShop_ReturnsOnlyMatchingItems()
    {
        var matchingItem = CreateShopItem(1, TestShop);
        var nonMatchingItem = CreateShopItem(2, SecondShop);
        _shopItemRepository.GetAsync().Returns(Task.FromResult<IEnumerable<ShopItem>>(new List<ShopItem> { matchingItem, nonMatchingItem }));

        var result = (await _shopItemService.GetItemsByShopIdAsync(DefaultShopId)).ToList();

        Assert.That(result, Contains.Item(matchingItem));
        Assert.That(result, Does.Not.Contain(nonMatchingItem));
    }

    [Test]
    public async Task GetItemsByShopIdAsync_NoItemsMatchShopId_ReturnsEmptyCollection()
    {
        _shopItemRepository.GetAsync().Returns(Task.FromResult<IEnumerable<ShopItem>>(new List<ShopItem>()));

        var result = await _shopItemService.GetItemsByShopIdAsync(DefaultShopId);

        Assert.That(result, Is.Empty);
    }

    // --- SearchItemsByNameAsync ---

    [Test]
    public async Task SearchItemsByNameAsync_NullSearchText_ReturnsAllItemsInShop()
    {
        var item = CreateShopItem(DefaultShopItemId, TestShop);
        _shopItemRepository.GetAsync().Returns(Task.FromResult<IEnumerable<ShopItem>>(new List<ShopItem> { item }));

        var result = (await _shopItemService.SearchItemsByNameAsync(DefaultShopId, null!)).ToList();

        Assert.That(result, Contains.Item(item));
    }

    [Test]
    public async Task SearchItemsByNameAsync_TextMatchesItemName_ReturnsMatchingItem()
    {
        var matchingItem = CreateShopItem(1, TestShop, name: "Apple Juice");
        var nonMatchingItem = CreateShopItem(2, TestShop, name: "Biscuit");
        _shopItemRepository.GetAsync().Returns(Task.FromResult<IEnumerable<ShopItem>>(new List<ShopItem> { matchingItem, nonMatchingItem }));

        var result = (await _shopItemService.SearchItemsByNameAsync(DefaultShopId, "Apple")).ToList();

        Assert.That(result, Contains.Item(matchingItem));
        Assert.That(result, Does.Not.Contain(nonMatchingItem));
    }

    [Test]
    public async Task SearchItemsByNameAsync_TextMatchesNoItems_ReturnsEmptyCollection()
    {
        var item = CreateShopItem(DefaultShopItemId, TestShop, name: "Apple Juice");
        _shopItemRepository.GetAsync().Returns(Task.FromResult<IEnumerable<ShopItem>>(new List<ShopItem> { item }));

        var result = await _shopItemService.SearchItemsByNameAsync(DefaultShopId, "XYZ");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task SearchItemsByNameAsync_SearchIsCaseInsensitive_ReturnsMatchingItems()
    {
        var item = CreateShopItem(DefaultShopItemId, TestShop, name: "Apple Juice");
        _shopItemRepository.GetAsync().Returns(Task.FromResult<IEnumerable<ShopItem>>(new List<ShopItem> { item }));

        var result = (await _shopItemService.SearchItemsByNameAsync(DefaultShopId, "APPLE")).ToList();

        Assert.That(result, Contains.Item(item));
    }

    // --- AddShopItemAsync ---

    [Test]
    public void AddShopItemAsync_NullShop_ThrowsArgumentException()
    {
        var item = new ShopItem(0, DefaultQuantity, DefaultPrice, null!, string.Empty, DefaultItemName, string.Empty);

        Assert.ThrowsAsync<ArgumentException>(() => _shopItemService.AddShopItemAsync(item));
    }

    [Test]
    public void AddShopItemAsync_ShopWithZeroId_ThrowsArgumentException()
    {
        var item = CreateShopItem(0, InvalidShop);

        Assert.ThrowsAsync<ArgumentException>(() => _shopItemService.AddShopItemAsync(item));
    }

    [Test]
    public void AddShopItemAsync_NegativeQuantity_ThrowsArgumentException()
    {
        var item = CreateShopItem(0, TestShop, quantity: NegativeQuantity);

        Assert.ThrowsAsync<ArgumentException>(() => _shopItemService.AddShopItemAsync(item));
    }

    [Test]
    public void AddShopItemAsync_ZeroPrice_ThrowsArgumentException()
    {
        var item = CreateShopItem(0, TestShop, price: ZeroPrice);

        Assert.ThrowsAsync<ArgumentException>(() => _shopItemService.AddShopItemAsync(item));
    }

    [Test]
    public void AddShopItemAsync_NegativePrice_ThrowsArgumentException()
    {
        var item = CreateShopItem(0, TestShop, price: NegativePrice);

        Assert.ThrowsAsync<ArgumentException>(() => _shopItemService.AddShopItemAsync(item));
    }

    [Test]
    public void AddShopItemAsync_EmptyName_ThrowsArgumentException()
    {
        var item = CreateShopItem(0, TestShop, name: string.Empty);

        Assert.ThrowsAsync<ArgumentException>(() => _shopItemService.AddShopItemAsync(item));
    }

    [Test]
    public void AddShopItemAsync_WhitespaceName_ThrowsArgumentException()
    {
        var item = CreateShopItem(0, TestShop, name: WhitespaceString);

        Assert.ThrowsAsync<ArgumentException>(() => _shopItemService.AddShopItemAsync(item));
    }

    [Test]
    public async Task AddShopItemAsync_ValidItem_CallsRepositoryAdd()
    {
        var item = CreateShopItem(0, TestShop);

        await _shopItemService.AddShopItemAsync(item);

        await _shopItemRepository.Received(1).AddAsync(item);
    }

    // --- UpdateShopItemAsync ---

    [Test]
    public void UpdateShopItemAsync_NullShop_ThrowsArgumentException()
    {
        var item = new ShopItem(DefaultShopItemId, DefaultQuantity, DefaultPrice, null!, string.Empty, DefaultItemName, string.Empty);

        Assert.ThrowsAsync<ArgumentException>(() => _shopItemService.UpdateShopItemAsync(item));
    }

    [Test]
    public void UpdateShopItemAsync_ShopWithZeroId_ThrowsArgumentException()
    {
        var item = CreateShopItem(DefaultShopItemId, InvalidShop);

        Assert.ThrowsAsync<ArgumentException>(() => _shopItemService.UpdateShopItemAsync(item));
    }

    [Test]
    public void UpdateShopItemAsync_NegativeQuantity_ThrowsArgumentException()
    {
        var item = CreateShopItem(DefaultShopItemId, TestShop, quantity: NegativeQuantity);

        Assert.ThrowsAsync<ArgumentException>(() => _shopItemService.UpdateShopItemAsync(item));
    }

    [Test]
    public void UpdateShopItemAsync_ZeroPrice_ThrowsArgumentException()
    {
        var item = CreateShopItem(DefaultShopItemId, TestShop, price: ZeroPrice);

        Assert.ThrowsAsync<ArgumentException>(() => _shopItemService.UpdateShopItemAsync(item));
    }

    [Test]
    public void UpdateShopItemAsync_EmptyName_ThrowsArgumentException()
    {
        var item = CreateShopItem(DefaultShopItemId, TestShop, name: string.Empty);

        Assert.ThrowsAsync<ArgumentException>(() => _shopItemService.UpdateShopItemAsync(item));
    }

    [Test]
    public async Task UpdateShopItemAsync_ValidItem_CallsRepositoryUpdate()
    {
        var item = CreateShopItem(DefaultShopItemId, TestShop);

        await _shopItemService.UpdateShopItemAsync(item);

        await _shopItemRepository.Received(1).UpdateAsync(item);
    }

    // --- RemoveShopItemAsync ---

    [Test]
    public async Task RemoveShopItemAsync_ValidShopItemId_CallsRepositoryDelete()
    {
        await _shopItemService.RemoveShopItemAsync(DefaultShopItemId);

        await _shopItemRepository.Received(1).DeleteAsync(DefaultShopItemId);
    }

    // --- GetItemsSortedByPriceAsync ---

    [Test]
    public void GetItemsSortedByPriceAsync_NullShop_ThrowsArgumentNullException()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _shopItemService.GetItemsSortedByPriceAsync(null!));
    }

    [Test]
    public async Task GetItemsSortedByPriceAsync_MultipleItems_ReturnsCheapestItemFirst()
    {
        var cheapItem = CreateShopItem(1, TestShop, name: "Cheap Item", price: CheapItemPrice);
        var expensiveItem = CreateShopItem(2, TestShop, name: "Expensive Item", price: ExpensiveItemPrice);
        _shopItemRepository.GetAsync().Returns(Task.FromResult<IEnumerable<ShopItem>>(new List<ShopItem> { expensiveItem, cheapItem }));

        var result = (await _shopItemService.GetItemsSortedByPriceAsync(TestShop)).ToList();

        Assert.That(result[0], Is.EqualTo(cheapItem));
        Assert.That(result[1], Is.EqualTo(expensiveItem));
    }

    // --- GetItemsSortedAlphabeticallyAsync ---

    [Test]
    public void GetItemsSortedAlphabeticallyAsync_NullShop_ThrowsArgumentNullException()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _shopItemService.GetItemsSortedAlphabeticallyAsync(null!));
    }

    [Test]
    public async Task GetItemsSortedAlphabeticallyAsync_MultipleItems_ReturnsFirstItemAlphabetically()
    {
        var secondItem = CreateShopItem(1, TestShop, name: "Biscuit");
        var firstItem = CreateShopItem(2, TestShop, name: "Apple Juice");
        _shopItemRepository.GetAsync().Returns(Task.FromResult<IEnumerable<ShopItem>>(new List<ShopItem> { secondItem, firstItem }));

        var result = (await _shopItemService.GetItemsSortedAlphabeticallyAsync(TestShop)).ToList();

        Assert.That(result[0], Is.EqualTo(firstItem));
        Assert.That(result[1], Is.EqualTo(secondItem));
    }
}
