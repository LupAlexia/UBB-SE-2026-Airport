using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface ICartRepository
{
    Task<IEnumerable<Cart>> GetAsync();
    Task<Cart?> GetByIdAsync(int cartId);
    Task AddAsync(Cart cart);
    Task DeleteAsync(int cartId);

    Task AddItemToCartAsync(int cartId, CartItem item);
    Task RemoveItemFromCartAsync(int cartId, int cartItemId);
    Task UpdateItemQuantityAsync(int cartId, int cartItemId, int quantity);
    Task ClearCartAsync(int cartId);
}