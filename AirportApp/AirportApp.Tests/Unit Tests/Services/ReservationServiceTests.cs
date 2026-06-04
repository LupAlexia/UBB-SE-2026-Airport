using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class ReservationServiceTests
{
    private const int DefaultReservationId = 1;
    private const int DefaultCartId = 10;
    private const int DefaultClientId = 1;
    private const int DefaultShopItemId = 1;
    private const int SecondShopItemId = 2;
    private const int DefaultCartItemId = 1;
    private const int SecondCartItemId = 2;
    private const int NonMatchingCartId = 99;
    private const int SufficientStockQuantity = 10;
    private const int CartItemQuantity = 3;
    private const int ExpectedDecrementedQuantity = 7;
    private const int InitialRestockQuantity = 2;
    private const int ExpectedRestockedQuantity = 5;
    private const float ShopItemPrice = 5.0f;

    private static readonly Manager TestManager = new Manager(1, "Test Manager", "manager@test.com", "0700000000");
    private static readonly Shop TestShop = new Shop(1, "Test Shop", "Duty Free", TestManager);

    private IReservationRepository _reservationRepository = null!;
    private IShopItemRepository _shopItemRepository = null!;
    private ICartRepository _cartRepository = null!;
    private ReservationService _reservationService = null!;

    [SetUp]
    public void SetUp()
    {
        _reservationRepository = Substitute.For<IReservationRepository>();
        _shopItemRepository = Substitute.For<IShopItemRepository>();
        _cartRepository = Substitute.For<ICartRepository>();
        _reservationService = new ReservationService(_reservationRepository, _shopItemRepository, _cartRepository);
    }

    private static ShopItem CreateShopItem(int shopItemId, int stockQuantity) =>
        new ShopItem(shopItemId, stockQuantity, ShopItemPrice, TestShop, string.Empty, "Item " + shopItemId, "Description");

    private static Cart CreateCartWithItems(ICollection<CartItem> cartItems) =>
        new Cart(DefaultCartId, new Client(DefaultClientId, "Test Client"), cartItems);

    private static Cart CreateEmptyCart() =>
        new Cart(DefaultCartId, new Client(DefaultClientId, "Test Client"), new List<CartItem>());

    [Test]
    public void ReserveCartAsync_NullReservation_ThrowsArgumentNullException()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _reservationService.ReserveCartAsync(null!));
    }

    [Test]
    public void ReserveCartAsync_ShopItemNotFoundInInventory_ThrowsInvalidOperationException()
    {
        var shopItem = CreateShopItem(DefaultShopItemId, SufficientStockQuantity);
        var cartItem = new CartItem(DefaultCartItemId, shopItem, CartItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        var reservation = new Reservation(cart, true, DateTime.Now);
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(null));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _reservationService.ReserveCartAsync(reservation));
    }

    [Test]
    public void ReserveCartAsync_InsufficientStock_ThrowsInvalidOperationException()
    {
        var shopItem = CreateShopItem(DefaultShopItemId, stockQuantity: 2);
        var cartItem = new CartItem(DefaultCartItemId, shopItem, quantity: 5);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        var reservation = new Reservation(cart, true, DateTime.Now);
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(CreateShopItem(DefaultShopItemId, stockQuantity: 2)));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _reservationService.ReserveCartAsync(reservation));
    }

    [Test]
    public async Task ReserveCartAsync_InsufficientStock_DoesNotCallRepositoryAdd()
    {
        var shopItem = CreateShopItem(DefaultShopItemId, stockQuantity: 2);
        var cartItem = new CartItem(DefaultCartItemId, shopItem, quantity: 5);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        var reservation = new Reservation(cart, true, DateTime.Now);
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(CreateShopItem(DefaultShopItemId, stockQuantity: 2)));

        try { await _reservationService.ReserveCartAsync(reservation); } catch (InvalidOperationException) { }

        await _reservationRepository.DidNotReceive().AddAsync(Arg.Any<Reservation>());
    }

    [Test]
    public async Task ReserveCartAsync_InsufficientStockOnSecondItem_DoesNotUpdateAnyShopItem()
    {
        var firstShopItem = CreateShopItem(DefaultShopItemId, stockQuantity: 5);
        var secondShopItem = CreateShopItem(SecondShopItemId, stockQuantity: 1);
        var firstCartItem = new CartItem(DefaultCartItemId, firstShopItem, CartItemQuantity);
        var secondCartItem = new CartItem(SecondCartItemId, secondShopItem, quantity: 10);
        var cart = CreateCartWithItems(new List<CartItem> { firstCartItem, secondCartItem });
        var reservation = new Reservation(cart, true, DateTime.Now);
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(CreateShopItem(DefaultShopItemId, stockQuantity: 5)));
        _shopItemRepository.GetByIdAsync(SecondShopItemId).Returns(Task.FromResult<ShopItem?>(CreateShopItem(SecondShopItemId, stockQuantity: 1)));

        try { await _reservationService.ReserveCartAsync(reservation); } catch (InvalidOperationException) { }

        await _shopItemRepository.DidNotReceive().UpdateAsync(Arg.Any<ShopItem>());
    }

    [Test]
    public async Task ReserveCartAsync_SufficientStock_CallsRepositoryAdd()
    {
        var shopItem = CreateShopItem(DefaultShopItemId, SufficientStockQuantity);
        var cartItem = new CartItem(DefaultCartItemId, shopItem, CartItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        var reservation = new Reservation(cart, true, DateTime.Now);
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(CreateShopItem(DefaultShopItemId, SufficientStockQuantity)));

        await _reservationService.ReserveCartAsync(reservation);

        await _reservationRepository.Received(1).AddAsync(reservation);
    }

    [Test]
    public async Task ReserveCartAsync_SufficientStock_DecrementsShopItemQuantity()
    {
        var shopItem = CreateShopItem(DefaultShopItemId, SufficientStockQuantity);
        var cartItem = new CartItem(DefaultCartItemId, shopItem, CartItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        var reservation = new Reservation(cart, true, DateTime.Now);
        var fetchedShopItem = CreateShopItem(DefaultShopItemId, SufficientStockQuantity);
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(fetchedShopItem));

        await _reservationService.ReserveCartAsync(reservation);

        Assert.That(fetchedShopItem.Quantity, Is.EqualTo(ExpectedDecrementedQuantity));
    }

    [Test]
    public async Task ReserveCartAsync_SufficientStock_CallsUpdateForEachCartItem()
    {
        var firstShopItem = CreateShopItem(DefaultShopItemId, SufficientStockQuantity);
        var secondShopItem = CreateShopItem(SecondShopItemId, SufficientStockQuantity);
        var firstCartItem = new CartItem(DefaultCartItemId, firstShopItem, CartItemQuantity);
        var secondCartItem = new CartItem(SecondCartItemId, secondShopItem, CartItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { firstCartItem, secondCartItem });
        var reservation = new Reservation(cart, true, DateTime.Now);
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(CreateShopItem(DefaultShopItemId, SufficientStockQuantity)));
        _shopItemRepository.GetByIdAsync(SecondShopItemId).Returns(Task.FromResult<ShopItem?>(CreateShopItem(SecondShopItemId, SufficientStockQuantity)));

        await _reservationService.ReserveCartAsync(reservation);

        await _shopItemRepository.Received(2).UpdateAsync(Arg.Any<ShopItem>());
    }

    [Test]
    public async Task ReserveCartAsync_EmptyCart_CallsRepositoryAdd()
    {
        var reservation = new Reservation(CreateEmptyCart(), true, DateTime.Now);

        await _reservationService.ReserveCartAsync(reservation);

        await _reservationRepository.Received(1).AddAsync(reservation);
    }

    [Test]
    public async Task ReserveCartAsync_EmptyCart_DoesNotUpdateAnyShopItem()
    {
        var reservation = new Reservation(CreateEmptyCart(), true, DateTime.Now);

        await _reservationService.ReserveCartAsync(reservation);

        await _shopItemRepository.DidNotReceive().UpdateAsync(Arg.Any<ShopItem>());
    }

    [Test]
    public async Task GetActiveReservationForCartAsync_NoReservationsExist_ReturnsNull()
    {
        _reservationRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Reservation>>(new List<Reservation>()));

        var result = await _reservationService.GetActiveReservationForCartAsync(DefaultCartId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetActiveReservationForCartAsync_NoReservationMatchesCartId_ReturnsNull()
    {
        var reservation = new Reservation(DefaultReservationId, CreateEmptyCart(), true, DateTime.Now);
        _reservationRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Reservation>>(new List<Reservation> { reservation }));

        var result = await _reservationService.GetActiveReservationForCartAsync(NonMatchingCartId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetActiveReservationForCartAsync_ReservationMatchesCartButInactive_ReturnsNull()
    {
        var reservation = new Reservation(DefaultReservationId, CreateEmptyCart(), false, DateTime.Now);
        _reservationRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Reservation>>(new List<Reservation> { reservation }));

        var result = await _reservationService.GetActiveReservationForCartAsync(DefaultCartId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetActiveReservationForCartAsync_ActiveReservationMatchesCart_ReturnsReservation()
    {
        var reservation = new Reservation(DefaultReservationId, CreateEmptyCart(), true, DateTime.Now);
        _reservationRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Reservation>>(new List<Reservation> { reservation }));

        var result = await _reservationService.GetActiveReservationForCartAsync(DefaultCartId);

        Assert.That(result, Is.EqualTo(reservation));
    }

    [Test]
    public async Task GetActiveReservationForCartAsync_MultipleReservations_ReturnsActiveMatchingOne()
    {
        var firstCart = CreateEmptyCart();
        var secondCart = new Cart(20, new Client(2, "Other Client"), new List<CartItem>());
        var inactiveReservation = new Reservation(1, firstCart, false, DateTime.Now);
        var activeReservation = new Reservation(2, firstCart, true, DateTime.Now);
        var otherCartReservation = new Reservation(3, secondCart, true, DateTime.Now);
        _reservationRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Reservation>>(
            new List<Reservation> { inactiveReservation, activeReservation, otherCartReservation }));

        var result = await _reservationService.GetActiveReservationForCartAsync(DefaultCartId);

        Assert.That(result, Is.EqualTo(activeReservation));
    }

    [Test]
    public async Task CancelReservationAsync_ReservationNotFound_DoesNotCallRepositoryUpdate()
    {
        _reservationRepository.GetByIdAsync(DefaultReservationId).Returns(Task.FromResult<Reservation?>(null));

        await _reservationService.CancelReservationAsync(DefaultReservationId);

        await _reservationRepository.DidNotReceive().UpdateAsync(Arg.Any<Reservation>());
    }

    [Test]
    public async Task CancelReservationAsync_ReservationInactive_DoesNotCallRepositoryUpdate()
    {
        var reservation = new Reservation(DefaultReservationId, CreateEmptyCart(), false, DateTime.Now);
        _reservationRepository.GetByIdAsync(DefaultReservationId).Returns(Task.FromResult<Reservation?>(reservation));

        await _reservationService.CancelReservationAsync(DefaultReservationId);

        await _reservationRepository.DidNotReceive().UpdateAsync(Arg.Any<Reservation>());
    }

    [Test]
    public async Task CancelReservationAsync_ReservationInactive_DoesNotRestockItems()
    {
        var shopItem = CreateShopItem(DefaultShopItemId, SufficientStockQuantity);
        var cartItem = new CartItem(DefaultCartItemId, shopItem, CartItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        var reservation = new Reservation(DefaultReservationId, cart, false, DateTime.Now);
        _reservationRepository.GetByIdAsync(DefaultReservationId).Returns(Task.FromResult<Reservation?>(reservation));

        await _reservationService.CancelReservationAsync(DefaultReservationId);

        await _shopItemRepository.DidNotReceive().UpdateAsync(Arg.Any<ShopItem>());
    }

    [Test]
    public async Task CancelReservationAsync_ReservationInactive_DoesNotClearCart()
    {
        var reservation = new Reservation(DefaultReservationId, CreateEmptyCart(), false, DateTime.Now);
        _reservationRepository.GetByIdAsync(DefaultReservationId).Returns(Task.FromResult<Reservation?>(reservation));

        await _reservationService.CancelReservationAsync(DefaultReservationId);

        await _cartRepository.DidNotReceive().ClearCartAsync(Arg.Any<int>());
    }

    [Test]
    public async Task CancelReservationAsync_ReservationActive_RestocksShopItems()
    {
        var shopItem = CreateShopItem(DefaultShopItemId, InitialRestockQuantity);
        var cartItem = new CartItem(DefaultCartItemId, shopItem, CartItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { cartItem });
        var reservation = new Reservation(DefaultReservationId, cart, true, DateTime.Now);
        _reservationRepository.GetByIdAsync(DefaultReservationId).Returns(Task.FromResult<Reservation?>(reservation));
        var fetchedShopItem = CreateShopItem(DefaultShopItemId, InitialRestockQuantity);
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(fetchedShopItem));

        await _reservationService.CancelReservationAsync(DefaultReservationId);

        Assert.That(fetchedShopItem.Quantity, Is.EqualTo(ExpectedRestockedQuantity));
    }

    [Test]
    public async Task CancelReservationAsync_ReservationActive_CallsUpdateForEachCartItem()
    {
        var firstShopItem = CreateShopItem(DefaultShopItemId, InitialRestockQuantity);
        var secondShopItem = CreateShopItem(SecondShopItemId, InitialRestockQuantity);
        var firstCartItem = new CartItem(DefaultCartItemId, firstShopItem, CartItemQuantity);
        var secondCartItem = new CartItem(SecondCartItemId, secondShopItem, CartItemQuantity);
        var cart = CreateCartWithItems(new List<CartItem> { firstCartItem, secondCartItem });
        var reservation = new Reservation(DefaultReservationId, cart, true, DateTime.Now);
        _reservationRepository.GetByIdAsync(DefaultReservationId).Returns(Task.FromResult<Reservation?>(reservation));
        _shopItemRepository.GetByIdAsync(DefaultShopItemId).Returns(Task.FromResult<ShopItem?>(CreateShopItem(DefaultShopItemId, InitialRestockQuantity)));
        _shopItemRepository.GetByIdAsync(SecondShopItemId).Returns(Task.FromResult<ShopItem?>(CreateShopItem(SecondShopItemId, InitialRestockQuantity)));

        await _reservationService.CancelReservationAsync(DefaultReservationId);

        await _shopItemRepository.Received(2).UpdateAsync(Arg.Any<ShopItem>());
    }

    [Test]
    public async Task CancelReservationAsync_ReservationActive_ClearsReservationCart()
    {
        var reservation = new Reservation(DefaultReservationId, CreateEmptyCart(), true, DateTime.Now);
        _reservationRepository.GetByIdAsync(DefaultReservationId).Returns(Task.FromResult<Reservation?>(reservation));

        await _reservationService.CancelReservationAsync(DefaultReservationId);

        await _cartRepository.Received(1).ClearCartAsync(DefaultCartId);
    }

    [Test]
    public async Task CancelReservationAsync_ReservationActive_SetsReservationToInactive()
    {
        var reservation = new Reservation(DefaultReservationId, CreateEmptyCart(), true, DateTime.Now);
        _reservationRepository.GetByIdAsync(DefaultReservationId).Returns(Task.FromResult<Reservation?>(reservation));

        await _reservationService.CancelReservationAsync(DefaultReservationId);

        Assert.That(reservation.Active, Is.False);
    }

    [Test]
    public async Task CancelReservationAsync_ReservationActive_CallsRepositoryUpdate()
    {
        var reservation = new Reservation(DefaultReservationId, CreateEmptyCart(), true, DateTime.Now);
        _reservationRepository.GetByIdAsync(DefaultReservationId).Returns(Task.FromResult<Reservation?>(reservation));

        await _reservationService.CancelReservationAsync(DefaultReservationId);

        await _reservationRepository.Received(1).UpdateAsync(reservation);
    }
}