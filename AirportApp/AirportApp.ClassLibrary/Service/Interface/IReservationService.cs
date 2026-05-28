using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IReservationService
{
    Task<IEnumerable<Reservation>> GetAllReservationsAsync();
    Task<Reservation?> GetReservationByIdAsync(int reservationId);
    Task ReserveCartAsync(int cartId);
    Task CancelReservationAsync(int reservationId);
    Task<Reservation?> GetActiveReservationForCartAsync(int cartId);
}
