using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IReviewService
{
    Task<IEnumerable<Review>> GetAllAsync();
    Task<Review?> GetByIdAsync(int reviewId);
    Task AddAsync(Review review);
    Task UpdateAsync(Review review);
    Task DeleteAsync(int reviewId);
}
