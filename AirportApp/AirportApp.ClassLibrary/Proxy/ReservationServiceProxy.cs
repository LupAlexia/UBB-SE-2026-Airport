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
        await PostAsync(BaseUrl, reservation);
    }

    public async Task CancelReservationAsync(int reservationId)
    {
        await PostAsync<object>($"{BaseUrl}/{reservationId}/cancel", null!);
    }

    public async Task<Reservation?> GetActiveReservationForCartAsync(int cartId)
    {
        return await GetOptionalAsync<Reservation>($"{BaseUrl}/cart/{cartId}/active");
    }

    public async Task DeleteReservationAsync(int reservationId)
    {
        await DeleteAsync($"{BaseUrl}/{reservationId}");
    }
}
