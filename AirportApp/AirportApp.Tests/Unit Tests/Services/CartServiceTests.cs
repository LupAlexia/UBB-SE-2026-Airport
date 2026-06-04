using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class CartServiceTests
{
    private const int DefaultCartId = 1;
    private const int DefaultClientId = 1;
    private const int DefaultShopItemId = 1;
    private const int DefaultCartItemId = 1;
    private const int UnknownCartItemId = 999;
    private const int SufficientStock = 10;
    private const int NewItemQuantity = 3;
    private const int ExistingItemQuantity = 4;
    private const int CombinedQuantity = 7;
    private const int QuantityOfOne = 1;
    private const float ShopItemPrice = 5.0f;
    private const float ExpectedTotalPrice = 15.0f;

    private static readonly Manager TestManager = new Manager(1, "Test Manager", "manager@test.com", "0700000000");
    private static readonly Shop TestShop = new Shop(1, "Test Shop", "Duty Free", TestManager);

    private ICartRepository _cartRepository = null!;
    private IClientRepository _clientRepository = null!;
    private IShopItemRepository _shopItemRepository = null!;
    private CartService _cartService = null!;

    [SetUp]
    public void SetUp()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _clientRepository = Substitute.For<IClientRepository>();
        _shopItemRepository = Substitute.For<IShopItemRepository>();
        _cartService = new CartService(_cartRepository, _clientRepository, _shopItemRepository);
    }

    private static ShopItem CreateShopItem(int stockQuantity = SufficientStock) =>
        new ShopItem(DefaultShopItemId, stockQuantity, ShopItemPrice, TestShop, string.Empty, "Test Item", "Test Description");

    private static Client CreateTestClient() => new Client(DefaultClientId, "Test Client");

    private static Cart CreateEmptyCart() =>
        new Cart(DefaultCartId, CreateTestClient(), new List<CartItem>());

    private static Cart CreateCartWithItems(ICollection<CartItem> cartItems) =>
        new Cart(DefaultCartId, CreateTestClient(), cartItems);

    [Test]
    public async Task GetOrCreateCartAsync_CartAlreadyExistsForClient_ReturnsExistingCart()
    {
        var existingCart = CreateEmptyCart();
        _cartRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Cart>>(new List<Cart> { existingCart }));

        var result = await _cartService.GetOrCreateCartAsync(DefaultClientId);

        Assert.That(result, Is.EqualTo(existingCart));
    }

    [Test]
    public async Task GetOrCreateCartAsync_CartAlreadyExistsForClient_DoesNotAddNewCart()
    {
        var existingCart = CreateEmptyCart();
        _cartRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Cart>>(new List<Cart> { existingCart }));

        await _cartService.GetOrCreateCartAsync(DefaultClientId);

        await _cartRepository.DidNotReceive().AddAsync(Arg.Any<Cart>());
    }

    [Test]
    public async Task GetOrCreateCartAsync_NoCartExistsForClient_AddsNewCartToRepository()
    {
        _cartRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Cart>>(new List<Cart>()));
        _clientRepository.GetByIdAsync(DefaultClientId).Returns(Task.FromResult<Client?>(CreateTestClient()));

        await _cartService.GetOrCreateCartAsync(DefaultClientId);

        await _cartRepository.Received(1).AddAsync(Arg.Any<Cart>());
    }

    [Test]
    public async Task GetOrCreateCartAsync_ClientNotFoundInRepository_AddsCartWithFallbackClient()
    {
        _cartRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Cart>>(new List<Cart>()));
        _clientRepository.GetByIdAsync(DefaultClientId).Returns(Task.FromResult<Client?>(null));

        await _cartService.GetOrCreateCartAsync(DefaultClientId);

        await _cartRepository.Received(1).AddAsync(Arg.Is<Cart>(cart => cart.Client.Id == DefaultClientId));
    }

    [Test]
    public void AddItemToCartAsync_CartNotFound_ThrowsInvalidOperationException()
    {
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(null));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cartService.AddItemToCartAsync(DefaultCartId, DefaultShopItemId, NewItemQuantity));
    }

    [Test]
    public void AddItemToCartAsync_ShopItemNotFound_ThrowsInvalidOperationException()
    {
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(CreateEmptyCart()));
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(null));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cartService.AddItemToCartAsync(DefaultCartId, DefaultShopItemId, NewItemQuantity));
    }

    [Test]
    public void AddItemToCartAsync_QuantityExceedsStock_ThrowsInvalidOperationException()
    {
        var shopItem = CreateShopItem(stockQuantity: 2);
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(CreateEmptyCart()));
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(shopItem));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cartService.AddItemToCartAsync(DefaultCartId, DefaultShopItemId, 5));
    }

    [Test]
    public void AddItemToCartAsync_CombinedQuantityExceedsStock_ThrowsInvalidOperationException()
    {
        var shopItem = CreateShopItem(stockQuantity: 5);
        var existingCartItem = new CartItem(DefaultCartItemId, shopItem, ExistingItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { existingCartItem });
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(cart));
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(shopItem));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cartService.AddItemToCartAsync(DefaultCartId, DefaultShopItemId, 4));
    }

    [Test]
    public async Task AddItemToCartAsync_NewItemWithSufficientStock_AddsItemToRepository()
    {
        var shopItem = CreateShopItem();
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(CreateEmptyCart()));
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(shopItem));

        await _cartService.AddItemToCartAsync(DefaultCartId, DefaultShopItemId, NewItemQuantity);

        await _cartRepository.Received(1).AddItemToCartAsync(DefaultCartId, Arg.Any<CartItem>());
    }

    [Test]
    public async Task AddItemToCartAsync_ExistingItemWithSufficientStock_UpdatesCombinedQuantityInRepository()
    {
        var shopItem = CreateShopItem();
        var existingCartItem = new CartItem(DefaultCartItemId, shopItem, ExistingItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { existingCartItem });
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(cart));
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(shopItem));

        await _cartService.AddItemToCartAsync(DefaultCartId, DefaultShopItemId, NewItemQuantity);

        await _cartRepository.Received(1).UpdateItemQuantityAsync(DefaultCartId, DefaultCartItemId, CombinedQuantity);
    }

    [Test]
    public void UpdateItemQuantityAsync_CartNotFound_ThrowsInvalidOperationException()
    {
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(null));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cartService.UpdateItemQuantityAsync(DefaultCartId, DefaultCartItemId, NewItemQuantity));
    }

    [Test]
    public void UpdateItemQuantityAsync_CartItemNotFound_ThrowsInvalidOperationException()
    {
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(CreateEmptyCart()));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cartService.UpdateItemQuantityAsync(DefaultCartId, DefaultCartItemId, NewItemQuantity));
    }

    [Test]
    public void UpdateItemQuantityAsync_NewQuantityExceedsStock_ThrowsInvalidOperationException()
    {
        var shopItem = CreateShopItem(stockQuantity: 5);
        var cartItem = new CartItem(DefaultCartItemId, shopItem, ExistingItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(cart));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cartService.UpdateItemQuantityAsync(DefaultCartId, DefaultCartItemId, 10));
    }

    [Test]
    public async Task UpdateItemQuantityAsync_NewQuantityWithinStock_UpdatesQuantityInRepository()
    {
        var shopItem = CreateShopItem();
        var cartItem = new CartItem(DefaultCartItemId, shopItem, ExistingItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(cart));

        await _cartService.UpdateItemQuantityAsync(DefaultCartId, DefaultCartItemId, NewItemQuantity);

        await _cartRepository.Received(1).UpdateItemQuantityAsync(DefaultCartId, DefaultCartItemId, NewItemQuantity);
    }

    [Test]
    public void DecreaseItemQuantityAsync_CartNotFound_ThrowsInvalidOperationException()
    {
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(null));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cartService.DecreaseItemQuantityAsync(DefaultCartId, DefaultCartItemId));
    }

    [Test]
    public void DecreaseItemQuantityAsync_CartItemNotFound_ThrowsInvalidOperationException()
    {
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(CreateEmptyCart()));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cartService.DecreaseItemQuantityAsync(DefaultCartId, DefaultCartItemId));
    }

    [Test]
    public async Task DecreaseItemQuantityAsync_QuantityGreaterThanOne_DecreasesQuantityByOneInRepository()
    {
        var cartItem = new CartItem(DefaultCartItemId, CreateShopItem(), ExistingItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(cart));

        await _cartService.DecreaseItemQuantityAsync(DefaultCartId, DefaultCartItemId);

        await _cartRepository.Received(1).UpdateItemQuantityAsync(DefaultCartId, DefaultCartItemId, ExistingItemQuantity - 1);
    }

    [Test]
    public async Task DecreaseItemQuantityAsync_QuantityIsOne_RemovesItemFromRepository()
    {
        var cartItem = new CartItem(DefaultCartItemId, CreateShopItem(), QuantityOfOne);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(cart));

        await _cartService.DecreaseItemQuantityAsync(DefaultCartId, DefaultCartItemId);

        await _cartRepository.Received(1).RemoveItemFromCartAsync(DefaultCartId, DefaultCartItemId);
    }

    [Test]
    public async Task GetCartTotalAsync_CartNotFound_ReturnsZero()
    {
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(null));

        var total = await _cartService.GetCartTotalAsync(DefaultCartId);

        Assert.That(total, Is.EqualTo(0));
    }

    [Test]
    public async Task GetCartTotalAsync_CartWithItems_ReturnsCorrectTotal()
    {
        var cartItem = new CartItem(DefaultCartItemId, CreateShopItem(), NewItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(cart));

        var total = await _cartService.GetCartTotalAsync(DefaultCartId);

        Assert.That(total, Is.EqualTo(ExpectedTotalPrice).Within(0.001));
    }

    [Test]
    public async Task IsLastCartItemAsync_CartNotFound_ReturnsFalse()
    {
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(null));

        var result = await _cartService.IsLastCartItemAsync(DefaultCartId, DefaultCartItemId);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsLastCartItemAsync_CartItemNotFound_ReturnsFalse()
    {
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(CreateEmptyCart()));

        var result = await _cartService.IsLastCartItemAsync(DefaultCartId, UnknownCartItemId);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsLastCartItemAsync_QuantityIsOne_ReturnsTrue()
    {
        var cartItem = new CartItem(DefaultCartItemId, CreateShopItem(), QuantityOfOne);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(cart));

        var result = await _cartService.IsLastCartItemAsync(DefaultCartId, DefaultCartItemId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsLastCartItemAsync_QuantityGreaterThanOne_ReturnsFalse()
    {
        var cartItem = new CartItem(DefaultCartItemId, CreateShopItem(), ExistingItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(cart));

        var result = await _cartService.IsLastCartItemAsync(DefaultCartId, DefaultCartItemId);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetCartItemsAsync_CartNotFound_ReturnsEmptyCollection()
    {
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(null));

        var result = await _cartService.GetCartItemsAsync(DefaultCartId);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetCartItemsAsync_CartWithItems_ReturnsAllCartItems()
    {
        var cartItem1 = new CartItem(1, CreateShopItem(), NewItemQuantity);
        var cartItem2 = new CartItem(2, CreateShopItem(), ExistingItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem1, cartItem2 });
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(cart));

        var result = await _cartService.GetCartItemsAsync(DefaultCartId);

        Assert.That(result, Is.EquivalentTo(new[] { cartItem1, cartItem2 }));
    }

    [Test]
    public void RemoveItemFromCartAsync_CartNotFound_ThrowsKeyNotFoundException()
    {
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(null));

        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _cartService.RemoveItemFromCartAsync(DefaultCartId, DefaultCartItemId));
    }

    [Test]
    public void RemoveItemFromCartAsync_CartItemNotFound_ThrowsKeyNotFoundException()
    {
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(CreateEmptyCart()));

        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _cartService.RemoveItemFromCartAsync(DefaultCartId, DefaultCartItemId));
    }

    [Test]
    public async Task RemoveItemFromCartAsync_ValidCartAndItem_CallsRepositoryRemove()
    {
        var cartItem = new CartItem(DefaultCartItemId, CreateShopItem(), NewItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        _cartRepository.GetByIdAsync(DefaultCartId).Returns(Task.FromResult<Cart?>(cart));

        await _cartService.RemoveItemFromCartAsync(DefaultCartId, DefaultCartItemId);

        await _cartRepository.Received(1).RemoveItemFromCartAsync(DefaultCartId, DefaultCartItemId);
    }
}
