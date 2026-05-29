using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class CartService(ICartRepository cartRepository, IClientRepository clientRepository, IShopItemRepository shopItemRepository) : ICartService
{
    public async Task<Cart?> GetCartByIdAsync(int cartId)
    {
        return await cartRepository.GetByIdAsync(cartId);
    }

    public async Task<Cart> GetOrCreateCartAsync(int clientId)
    {
        var allCarts = await cartRepository.GetAsync();
        var existing = allCarts.FirstOrDefault(c => c.Client?.Id == clientId);
        if (existing is not null)
            return existing;

        var client = await clientRepository.GetByIdAsync(clientId) ?? new Client(clientId, "Current Client");
        var cart = new Cart(0, client, new List<CartItem>());
        await cartRepository.AddAsync(cart);

        var created = (await cartRepository.GetAsync()).FirstOrDefault(c => c.Client?.Id == clientId);
        return created ?? cart;
    }

    public async Task AddItemToCartAsync(int cartId, int shopItemId, int quantity)
    {
        var cart = await cartRepository.GetByIdAsync(cartId)
            ?? throw new InvalidOperationException($"Cart {cartId} not found.");
        var shopItem = await shopItemRepository.GetByIdAsync(shopItemId)
            ?? throw new InvalidOperationException($"ShopItem {shopItemId} not found.");

        var existing = cart.CartItems.FirstOrDefault(ci => ci.ShopItem?.Id == shopItemId);
        int currentQty = existing?.Quantity ?? 0;

        if (currentQty + quantity > shopItem.Quantity)
            throw new InvalidOperationException("Not enough stock available.");

        if (existing is not null)
        {
            await cartRepository.UpdateItemQuantityAsync(cartId, existing.Id, currentQty + quantity);
        }
        else
        {
            var newItem = new CartItem(0, shopItem, quantity);
            await cartRepository.AddItemToCartAsync(cartId, newItem);
        }
    }

    public async Task UpdateItemQuantityAsync(int cartId, int cartItemId, int newQuantity)
    {
        var cart = await cartRepository.GetByIdAsync(cartId)
            ?? throw new InvalidOperationException($"Cart {cartId} not found.");
        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId)
            ?? throw new InvalidOperationException($"CartItem {cartItemId} not found.");

        if (newQuantity > cartItem.ShopItem.Quantity)
            throw new InvalidOperationException("Quantity exceeds available stock.");

        await cartRepository.UpdateItemQuantityAsync(cartId, cartItemId, newQuantity);
    }

    public async Task DecreaseItemQuantityAsync(int cartId, int cartItemId)
    {
        var cart = await cartRepository.GetByIdAsync(cartId)
            ?? throw new InvalidOperationException($"Cart {cartId} not found.");
        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId)
            ?? throw new InvalidOperationException($"CartItem {cartItemId} not found.");

        if (cartItem.Quantity > 1)
            await cartRepository.UpdateItemQuantityAsync(cartId, cartItemId, cartItem.Quantity - 1);
        else
            await cartRepository.RemoveItemFromCartAsync(cartId, cartItemId);
    }

    public async Task ClearCartAsync(int cartId)
    {
        await cartRepository.ClearCartAsync(cartId);
    }

    public async Task<double> GetCartTotalAsync(int cartId)
    {
        var cart = await cartRepository.GetByIdAsync(cartId);
        if (cart is null) return 0;
        return cart.GetOverallPrice();
    }

    public async Task<bool> IsLastCartItemAsync(int cartId, int cartItemId)
    {
        var cart = await cartRepository.GetByIdAsync(cartId);
        if (cart is null) return false;
        var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
        return item?.Quantity == 1;
    }

    public async Task<IEnumerable<CartItem>> GetCartItemsAsync(int cartId)
    {
        var cart = await cartRepository.GetByIdAsync(cartId);
        return cart?.CartItems ?? Enumerable.Empty<CartItem>();
    }

    public async Task<IEnumerable<Cart>> GetAllCartsAsync()
    {
        return await cartRepository.GetAsync();
    }

    public async Task AddCartAsync(Cart cart)
    {
        await cartRepository.AddAsync(cart);
    }

    public async Task DeleteCartAsync(int cartId)
    {
        await cartRepository.DeleteAsync(cartId);
    }
    public async Task RemoveItemFromCartAsync(int cartId, int cartItemId)
    {
        Cart? cart = await cartRepository.GetByIdAsync(cartId);
        if (cart == null) throw new KeyNotFoundException($"Cart {cartId} not found.");
        CartItem? item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
        if (item == null) throw new KeyNotFoundException($"CartItem {cartItemId} not found in cart.");
        cart.CartItems.Remove(item);
        await cartRepository.UpdateAsync(cart);
    }
}
