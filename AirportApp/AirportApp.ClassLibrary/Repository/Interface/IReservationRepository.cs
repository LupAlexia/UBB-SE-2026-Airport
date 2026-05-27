using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetAsync();
    Task<Reservation?> GetByIdAsync(int reservationId);
    Task<int> AddAsync(Reservation reservation);
    Task UpdateAsync(Reservation reservation);
    Task DeleteAsync(int reservationId);
}