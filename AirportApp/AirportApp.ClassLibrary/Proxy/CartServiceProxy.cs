using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class CartServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), ICartService
{
    private const int MinimumCartItemQuantity = 1;
    private const string BaseUrl = "api/carts";

    public async Task<IEnumerable<Cart>> GetAllCartsAsync()
    {
        var list = await GetListAsync<CartDto>(BaseUrl);
        return list.Select(MapCart).ToList();
    }

    public async Task<Cart?> GetCartByIdAsync(int cartId)
    {
        var cartDto = await GetOptionalAsync<CartDto>($"{BaseUrl}/{cartId}");
        return cartDto is null ? null : MapCart(cartDto);
    }

    public async Task<Cart> GetOrCreateCartAsync(int clientId)
    {
        var list = await GetListAsync<CartDto>(BaseUrl);
        var carts = list.Select(MapCart).ToList();
        var existing = carts.FirstOrDefault(c => c.Client?.Id == clientId);
        if (existing is not null) return existing;

        var newCart = new Cart(
            0,
            new Client(clientId, "Current Client"),
            new List<CartItem>());

        await AddCartAsync(newCart);

        // fetch again to get generated id
        var refreshed = (await GetListAsync<CartDto>(BaseUrl)).Select(MapCart).FirstOrDefault(c => c.Client?.Id == clientId);
        return refreshed ?? newCart;
    }

    public async Task AddCartAsync(Cart cart)
    {
        await PostAsync(BaseUrl, MapCartRequest(cart));
    }

    public async Task DeleteCartAsync(int cartId)
    {
        await DeleteAsync($"{BaseUrl}/{cartId}");
    }

    public async Task AddItemToCartAsync(int cartId, int shopItemId, int quantity)
    {
        var payload = new CartItemRequest(0, shopItemId, quantity);
        await PostAsync($"{BaseUrl}/{cartId}/items", payload);
    }

    public async Task RemoveItemFromCartAsync(int cartId, int cartItemId)
    {
        await DeleteAsync($"{BaseUrl}/{cartId}/items/{cartItemId}");
    }

    public async Task UpdateItemQuantityAsync(int cartId, int cartItemId, int newQuantity)
    {
        await PutAsync($"{BaseUrl}/{cartId}/items/{cartItemId}/quantity", new UpdateCartItemQuantityRequest(newQuantity));
    }

    public async Task ClearCartAsync(int cartId)
    {
        await DeleteAsync($"{BaseUrl}/{cartId}/items");
    }

    public async Task DecreaseItemQuantityAsync(int cartId, int cartItemId)
    {
        var targetCart = await GetCartByIdAsync(cartId);
        if (targetCart is null) return;
        var itemToModify = FindItemInCartById(targetCart, cartItemId);
        if (itemToModify is null) return;

        if (itemToModify.Quantity > MinimumCartItemQuantity)
        {
            await UpdateItemQuantityAsync(cartId, cartItemId, itemToModify.Quantity - MinimumCartItemQuantity);
        }
        else
        {
            await RemoveItemFromCartAsync(cartId, cartItemId);
        }
    }

    public async Task<double> GetCartTotalAsync(int cartId)
    {
        var targetCart = await GetCartByIdAsync(cartId);
        return targetCart?.GetOverallPrice() ?? 0;
    }

    public async Task<bool> IsLastCartItemAsync(int cartId, int cartItemId)
    {
        var targetCart = await GetCartByIdAsync(cartId);
        if (targetCart is null) return false;
        var targetItem = FindItemInCartById(targetCart, cartItemId);
        return targetItem is not null && targetItem.Quantity == MinimumCartItemQuantity;
    }

    public async Task<IEnumerable<CartItem>> GetCartItemsAsync(int cartId)
    {
        var targetCart = await GetCartByIdAsync(cartId);
        return targetCart?.CartItems ?? [];
    }

    private static CartItem? FindItemInCartById(Cart cart, int cartItemId)
    {
        return cart.CartItems.FirstOrDefault(item => item.Id == cartItemId);
    }

    private static Cart MapCart(CartDto cart)
    {
        return new Cart(
            cart.Id,
            MapClient(cart.Client),
            cart.CartItems?.Select(MapCartItem).ToList() ?? []);
    }

    private static CartItem MapCartItem(CartItemDto cartItem)
    {
        return new CartItem(
            cartItem.Id,
            MapShopItem(cartItem.ShopItem),
            cartItem.Quantity);
    }

    private static CartRequest MapCartRequest(Cart cart)
    {
        return new CartRequest(
            cart.Id,
            new ClientRequest(cart.Client.Id, cart.Client.Name),
            cart.CartItems?.Select(MapCartItemRequest).ToList() ?? []);
    }

    private static CartItemRequest MapCartItemRequest(CartItem cartItem)
    {
        return new CartItemRequest(
            cartItem.Id,
            cartItem.ShopItem.Id,
            cartItem.Quantity);
    }

    private static ShopItem MapShopItem(ShopItemDto? shopItem)
    {
        if (shopItem is null) return new ShopItem();
        return new ShopItem(
            shopItem.Id,
            shopItem.Quantity,
            shopItem.Price,
            MapShop(shopItem.Shop),
            shopItem.Photo ?? string.Empty,
            shopItem.Name ?? string.Empty,
            shopItem.Description ?? string.Empty);
    }

    private static Shop MapShop(ShopDto? shop)
    {
        if (shop is null) return new Shop(string.Empty, string.Empty, MapManager(null));
        return new Shop(
            shop.Id,
            shop.Name ?? string.Empty,
            shop.Type ?? string.Empty,
            MapManager(shop.Manager));
    }

    private static Manager MapManager(ManagerDto? manager)
    {
        if (manager is null) return new Manager(0, string.Empty, string.Empty, string.Empty);
        return new Manager(
            manager.Id,
            manager.Name ?? string.Empty,
            manager.Email ?? string.Empty,
            manager.Phone ?? string.Empty);
    }

    private static Client MapClient(ClientDto? client)
    {
        if (client is null) return new Client(0, string.Empty);
        return new Client(client.Id, client.Name ?? string.Empty);
    }

    private sealed class CartDto
    {
        public int Id { get; set; }
        public ClientDto? Client { get; set; }
        public List<CartItemDto>? CartItems { get; set; }
    }

    private sealed class CartItemDto
    {
        public int Id { get; set; }
        public ShopItemDto? ShopItem { get; set; }
        public int Quantity { get; set; }
    }

    private sealed class ShopItemDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public ShopDto? Shop { get; set; }
        public string? Photo { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    private sealed class ShopDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public ManagerDto? Manager { get; set; }
    }

    private sealed class ManagerDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }

    private sealed class ClientDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private sealed record CartRequest(int Id, ClientRequest Client, List<CartItemRequest> CartItems);
    private sealed record CartItemRequest(int Id, int ShopItemId, int Quantity);
    private sealed record ClientRequest(int Id, string Name);
    private sealed record UpdateCartItemQuantityRequest(int Quantity);
}
