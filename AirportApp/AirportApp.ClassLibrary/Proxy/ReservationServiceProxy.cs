using System;
using System.Collections.Generic;
using System.Linq;
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
        var dtos = await GetListAsync<ReservationDto>(BaseUrl);
        return dtos.Select(MapReservation).ToList();
    }

    public async Task<Reservation?> GetReservationByIdAsync(int reservationId)
    {
        var dto = await GetOptionalAsync<ReservationDto>($"{BaseUrl}/{reservationId}");
        return dto is null ? null : MapReservation(dto);
    }

    public async Task ReserveCartAsync(Reservation reservation)
    {
        var request = new ReserveCartRequest(
            reservation.ReservationCart.Id,
            reservation.Active,
            reservation.ReservationDate,
            reservation.ReservationCart.CartItems
                .Select(item => new CartItemRequest(item.Id, item.ShopItem.Id, item.Quantity))
                .ToList());

        await PostAsync($"{BaseUrl}/reserve", request);
    }

    public async Task CancelReservationAsync(int reservationId)
    {
        await PutAsync<object>($"{BaseUrl}/{reservationId}/cancel", null!);
    }

    public async Task<Reservation?> GetActiveReservationForCartAsync(int cartId)
    {
        var dto = await GetOptionalAsync<ReservationDto>($"{BaseUrl}/cart/{cartId}/active");
        return dto is null ? null : MapReservation(dto);
    }

    public async Task DeleteReservationAsync(int reservationId)
    {
        await DeleteAsync($"{BaseUrl}/{reservationId}");
    }

    private static Reservation MapReservation(ReservationDto dto)
    {
        return new Reservation(
            dto.Id,
            new Cart(dto.ReservationCart?.Id ?? 0, new Client(0, string.Empty), new List<CartItem>()),
            dto.Active,
            dto.ReservationDate);
    }

    private sealed class ReservationDto
    {
        public int Id { get; set; }
        public ReservationCartDto? ReservationCart { get; set; }
        public bool Active { get; set; }
        public DateTime ReservationDate { get; set; }
    }

    private sealed class ReservationCartDto
    {
        public int Id { get; set; }
    }

    private sealed record ReserveCartRequest(int CartId, bool Active, DateTime ReservationDate, List<CartItemRequest> CartItems);

    private sealed record CartItemRequest(int Id, int ShopItemId, int Quantity);
}
