using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAsync();
    Task<Review?> GetByIdAsync(int reviewId);
    Task<int> AddAsync(Review review);
    Task UpdateAsync(Review review);
    Task DeleteAsync(int reviewId);
}