using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class ReservationServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IReservationService
{
    private const string BaseUrl = "api/reservations";

    public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
    {
        return await GetListAsync<Reservation>(BaseUrl);
    }

    public async Task<Reservation?> GetReservationByIdAsync(int reservationId)
    {
        return await GetOptionalAsync<Reservation>($"{BaseUrl}/{reservationId}");
    }

    public async Task ReserveCartAsync(Reservation reservation)
    {
        if (reservation == null) throw new ArgumentNullException(nameof(reservation));

        // Map Reservation -> DTO expected by API (ReserveCartRequest)
        var cart = reservation.ReservationCart;
        var items = cart.CartItems?.Select(ci => new ReserveCartItemRequest(ci.Id, ci.ShopItem?.Id ?? 0, ci.Quantity)).ToList() ?? new List<ReserveCartItemRequest>();

        var payload = new ReserveCartRequest(cart.Id, reservation.Active, reservation.ReservationDate, items);

        await PostAsync($"{BaseUrl}/reserve", payload);
    }

    public async Task CancelReservationAsync(int reservationId)
    {
        await PutAsync($"{BaseUrl}/{reservationId}/cancel", new { });
    }

    public async Task<Reservation?> GetActiveReservationForCartAsync(int cartId)
    {
        return await GetOptionalAsync<Reservation>($"{BaseUrl}/cart/{cartId}/active");
    }

    public async Task DeleteReservationAsync(int reservationId)
    {
        await DeleteAsync($"{BaseUrl}/{reservationId}");
    }

    private sealed record ReserveCartItemRequest(int Id, int ShopItemId, int Quantity);
    private sealed record ReserveCartRequest(int CartId, bool Active, DateTime ReservationDate, List<ReserveCartItemRequest> CartItems);
}
