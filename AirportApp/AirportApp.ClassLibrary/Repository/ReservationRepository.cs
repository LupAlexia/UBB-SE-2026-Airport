using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class ReservationRepository(AppDbContext databaseContext) : IReservationRepository
{
    public async Task<IEnumerable<Reservation>> GetAsync()
    {
        return await databaseContext.Reservations
            .Include(reservation => reservation.ReservationCart)
                .ThenInclude(cart => cart.Client)
            .Include(reservation => reservation.ReservationCart)
                .ThenInclude(cart => cart.CartItems)
                    .ThenInclude(cartItem => cartItem.ShopItem)
                        .ThenInclude(shopItem => shopItem.Shop)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Reservation?> GetByIdAsync(int reservationId)
    {
        return await databaseContext.Reservations
            .Include(reservation => reservation.ReservationCart)
                .ThenInclude(cart => cart.Client)
            .Include(reservation => reservation.ReservationCart)
                .ThenInclude(cart => cart.CartItems)
                    .ThenInclude(cartItem => cartItem.ShopItem)
                        .ThenInclude(shopItem => shopItem.Shop)
            .FirstOrDefaultAsync(reservation => reservation.Id == reservationId);
    }

    public async Task<int> AddAsync(Reservation reservation)
    {
        if (reservation is null)
        {
            throw new ArgumentNullException(nameof(reservation));
        }

        if (reservation.ReservationCart is not null)
        {
            var existingCart = await databaseContext.Carts.FindAsync(reservation.ReservationCart.Id);
            if (existingCart is null)
            {
                throw new InvalidOperationException($"Cannot create reservation: Cart {reservation.ReservationCart.Id} does not exist.");
            }

            reservation.ReservationCart = existingCart;
        }

        reservation.Id = 0;
        databaseContext.Reservations.Add(reservation);
        await databaseContext.SaveChangesAsync();

        return reservation.Id;
    }

    public async Task UpdateAsync(Reservation reservation)
    {
        if (reservation is null)
        {
            throw new ArgumentNullException(nameof(reservation));
        }

        var existingReservation = await databaseContext.Reservations.FindAsync(reservation.Id);
        if (existingReservation is null)
        {
            return;
        }

        existingReservation.Active = reservation.Active;
        existingReservation.ReservationDate = reservation.ReservationDate;

        if (reservation.ReservationCart is not null)
        {
            var existingCart = await databaseContext.Carts.FindAsync(reservation.ReservationCart.Id);
            if (existingCart is not null)
            {
                existingReservation.ReservationCart = existingCart;
            }
        }

        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int reservationId)
    {
        var reservationToRemove = await databaseContext.Reservations.FindAsync(reservationId);

        if (reservationToRemove is null)
        {
            return;
        }

        databaseContext.Reservations.Remove(reservationToRemove);
        await databaseContext.SaveChangesAsync();
    }
}