using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ReservationService(
    IReservationRepository reservationRepository,
    IShopItemRepository shopItemRepository,
    ICartRepository cartRepository) : IReservationService
{
    private const string OutOfStockErrorMessageTemplate = "Not enough stock for '{0}'. Requested: {1}, Available: {2}";
    private const string MissingShopItemErrorMessage = "A product included in the reservation could not be found in the inventory.";

    public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
    {
        return await reservationRepository.GetAsync();
    }

    public async Task<Reservation?> GetReservationByIdAsync(int reservationId)
    {
        return await reservationRepository.GetByIdAsync(reservationId);
    }

    public async Task ReserveCartAsync(Reservation reservation)
    {
        if (reservation == null)
        {
            throw new ArgumentNullException(nameof(reservation));
        }

        var reservationCartItems = reservation.ReservationCart.CartItems;

        foreach (CartItem cartItem in reservationCartItems)
        {
            ShopItem? shopItem = await shopItemRepository.GetByIdAsync(cartItem.ShopItem.Id);

            if (shopItem == null)
            {
                throw new InvalidOperationException(MissingShopItemErrorMessage);
            }

            if (shopItem.Quantity < cartItem.Quantity)
            {
                string errorMessage = string.Format(
                    OutOfStockErrorMessageTemplate,
                    shopItem.Name,
                    cartItem.Quantity,
                    shopItem.Quantity);

                throw new InvalidOperationException(errorMessage);
            }
        }

        foreach (CartItem cartItem in reservationCartItems)
        {
            ShopItem shopItem = (await shopItemRepository.GetByIdAsync(cartItem.ShopItem.Id))!;
            shopItem.Quantity -= cartItem.Quantity;
            await shopItemRepository.UpdateAsync(shopItem);
        }

        await reservationRepository.AddAsync(reservation);
    }

    public async Task<Reservation?> GetActiveReservationForCartAsync(int cartId)
    {
        IEnumerable<Reservation> allReservations = await reservationRepository.GetAsync();

        foreach (Reservation reservation in allReservations)
        {
            if (reservation.ReservationCart.Id == cartId && reservation.Active)
            {
                return reservation;
            }
        }

        return null;
    }

    public async Task DeleteReservationAsync(int reservationId)
    {
        await reservationRepository.DeleteAsync(reservationId);
    }

    public async Task CancelReservationAsync(int reservationId)
    {
        Reservation? reservation = await reservationRepository.GetByIdAsync(reservationId);

        if (reservation == null)
        {
            return;
        }

        if (!reservation.Active)
        {
            return;
        }

        if (reservation.ReservationCart != null && reservation.ReservationCart.CartItems != null)
        {
            foreach (CartItem cartItem in reservation.ReservationCart.CartItems)
            {
                ShopItem? shopItem = await shopItemRepository.GetByIdAsync(cartItem.ShopItem.Id);
                if (shopItem != null)
                {
                    shopItem.Quantity += cartItem.Quantity;
                    await shopItemRepository.UpdateAsync(shopItem);
                }
            }
        }

        await cartRepository.ClearCartAsync(reservation.ReservationCart!.Id);
        reservation.Active = false;

        await reservationRepository.UpdateAsync(reservation);
    }
}
