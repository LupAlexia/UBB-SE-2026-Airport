using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class CartRepository(AppDbContext databaseContext) : ICartRepository
{
    private const string CartIdShadowProperty = "CartId";

    public async Task<IEnumerable<Cart>> GetAsync()
    {
        return await databaseContext.Carts
            .Include(cart => cart.Client)
            .Include(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.ShopItem)
                    .ThenInclude(shopItem => shopItem.Shop)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Cart?> GetByIdAsync(int cartId)
    {
        return await databaseContext.Carts
            .Include(cart => cart.Client)
            .Include(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.ShopItem)
                    .ThenInclude(shopItem => shopItem.Shop)
            .FirstOrDefaultAsync(cart => cart.Id == cartId);
    }

    public async Task AddAsync(Cart cart)
    {
        if (cart is null)
        {
            throw new ArgumentNullException(nameof(cart));
        }

        cart.Id = 0;

        if (cart.Client is not null)
        {
            var existingClient = await databaseContext.Clients.FindAsync(cart.Client.Id);
            if (existingClient is null)
            {
                databaseContext.Clients.Add(cart.Client);
            }
            else
            {
                cart.Client = existingClient;
            }
        }

        databaseContext.Carts.Add(cart);
        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int cartId)
    {
        var relatedItems = await databaseContext.CartItems
            .Where(cartItem => EF.Property<int>(cartItem, CartIdShadowProperty) == cartId)
            .ToListAsync();

        if (relatedItems.Count > 0)
        {
            databaseContext.CartItems.RemoveRange(relatedItems);
        }

        var cartToDelete = await databaseContext.Carts.FindAsync(cartId);
        if (cartToDelete is null)
        {
            return;
        }

        databaseContext.Carts.Remove(cartToDelete);
        await databaseContext.SaveChangesAsync();
    }

    public async Task AddItemToCartAsync(int cartId, CartItem itemToAdd)
    {
        if (itemToAdd is null)
        {
            throw new ArgumentNullException(nameof(itemToAdd));
        }

        var cart = await databaseContext.Carts
            .Include(cartInstance => cartInstance.CartItems)
            .FirstOrDefaultAsync(cartInstance => cartInstance.Id == cartId);

        if (cart is null)
        {
            throw new KeyNotFoundException($"Cart with ID {cartId} was not found in the database.");
        }

        if (itemToAdd.ShopItem is not null)
        {
            var existingShopItem = await databaseContext.ShopItems.FindAsync(itemToAdd.ShopItem.Id);
            if (existingShopItem is null)
            {
                throw new KeyNotFoundException($"ShopItem with ID {itemToAdd.ShopItem.Id} was not found. Verify your seeded data match.");
            }

            itemToAdd.ShopItem = existingShopItem;
        }

        itemToAdd.Id = 0;
        cart.CartItems.Add(itemToAdd);
        await databaseContext.SaveChangesAsync();
    }

    public async Task RemoveItemFromCartAsync(int cartId, int cartItemId)
    {
        var itemToRemove = await databaseContext.CartItems.FindAsync(cartItemId);
        if (itemToRemove is null)
        {
            return;
        }

        databaseContext.CartItems.Remove(itemToRemove);
        await databaseContext.SaveChangesAsync();
    }

    public async Task UpdateItemQuantityAsync(int cartId, int cartItemId, int newQuantity)
    {
        var itemToUpdate = await databaseContext.CartItems.FindAsync(cartItemId);
        if (itemToUpdate is null)
        {
            return;
        }

        itemToUpdate.Quantity = newQuantity;
        await databaseContext.SaveChangesAsync();
    }

    public async Task ClearCartAsync(int cartId)
    {
        var itemsToClear = await databaseContext.CartItems
            .Where(cartItem => EF.Property<int>(cartItem, CartIdShadowProperty) == cartId)
            .ToListAsync();

        if (itemsToClear.Count == 0)
        {
            return;
        }

        databaseContext.CartItems.RemoveRange(itemsToClear);
        await databaseContext.SaveChangesAsync();
    }
}