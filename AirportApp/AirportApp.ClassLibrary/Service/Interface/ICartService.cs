using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface ICartService
{
    Task<Cart> GetOrCreateCartAsync(int clientId);
    Task AddItemToCartAsync(int cartId, int shopItemId, int quantity);
    Task UpdateItemQuantityAsync(int cartId, int cartItemId, int newQuantity);
    Task DecreaseItemQuantityAsync(int cartId, int cartItemId);
    Task ClearCartAsync(int cartId);
    Task<float> GetCartTotalAsync(int cartId);
    Task<bool> IsLastCartItemAsync(int cartId, int cartItemId);
    Task<IEnumerable<CartItem>> GetCartItemsAsync(int cartId);
    Task<IEnumerable<Cart>> GetAllCartsAsync();
    Task AddCartAsync(Cart cart);
    Task DeleteCartAsync(int cartId);
}
