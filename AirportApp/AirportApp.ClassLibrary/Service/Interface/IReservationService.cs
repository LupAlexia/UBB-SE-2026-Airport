using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IReservationService
{
    Task<IEnumerable<Reservation>> GetAllReservationsAsync();
    Task<Reservation?> GetReservationByIdAsync(int reservationId);
    Task ReserveCartAsync(Reservation reservation);
    Task CancelReservationAsync(int reservationId);
    Task<Reservation?> GetActiveReservationForCartAsync(int cartId);
    Task DeleteReservationAsync(int reservationId);
}
