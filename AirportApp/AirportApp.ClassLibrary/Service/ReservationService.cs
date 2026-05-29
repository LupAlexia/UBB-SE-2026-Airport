using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ReservationService(
    IReservationRepository reservationRepository,
    ICartRepository cartRepository,
    IShopItemRepository shopItemRepository) : IReservationService
{
    public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
    {
        return await reservationRepository.GetAsync();
    }

    public async Task<Reservation?> GetReservationByIdAsync(int reservationId)
    {
        return await reservationRepository.GetByIdAsync(reservationId);
    }

    public async Task ReserveCartAsync(int cartId)
    {
        var cart = await cartRepository.GetByIdAsync(cartId)
            ?? throw new InvalidOperationException($"Cart {cartId} not found.");

        foreach (var cartItem in cart.CartItems)
        {
            var shopItem = await shopItemRepository.GetByIdAsync(cartItem.ShopItem.Id)
                ?? throw new InvalidOperationException($"ShopItem {cartItem.ShopItem.Id} not found.");

            if (cartItem.Quantity > shopItem.Quantity)
                throw new InvalidOperationException($"Insufficient stock for '{shopItem.Name}'.");

            shopItem.Quantity -= cartItem.Quantity;
            await shopItemRepository.UpdateAsync(shopItem);
        }

        var reservation = new Reservation(cart, true, DateTime.UtcNow);
        await reservationRepository.AddAsync(reservation);
    }

    public async Task CancelReservationAsync(int reservationId)
    {
        var reservation = await reservationRepository.GetByIdAsync(reservationId)
            ?? throw new InvalidOperationException($"Reservation {reservationId} not found.");

        if (reservation.ReservationCart is not null)
        {
            var cart = await cartRepository.GetByIdAsync(reservation.ReservationCart.Id);
            if (cart is not null)
            {
                foreach (var cartItem in cart.CartItems)
                {
                    var shopItem = await shopItemRepository.GetByIdAsync(cartItem.ShopItem.Id);
                    if (shopItem is not null)
                    {
                        shopItem.Quantity += cartItem.Quantity;
                        await shopItemRepository.UpdateAsync(shopItem);
                    }
                }
                await cartRepository.ClearCartAsync(cart.Id);
            }
        }

        reservation.Active = false;
        await reservationRepository.UpdateAsync(reservation);
    }

    public async Task<Reservation?> GetActiveReservationForCartAsync(int cartId)
    {
        var allReservations = await reservationRepository.GetAsync();
        return allReservations.FirstOrDefault(r => r.ReservationCart?.Id == cartId && r.Active);
    }
    public async Task DeleteReservationAsync(int reservationId)
    {
        await reservationRepository.DeleteAsync(reservationId);
    }
}
