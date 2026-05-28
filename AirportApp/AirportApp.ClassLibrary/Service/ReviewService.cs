using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ReviewService(IReviewRepository reviewRepository) : IReviewService
{
    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        return await reviewRepository.GetAsync();
    }

    public async Task<Review?> GetByIdAsync(int reviewId)
    {
        return await reviewRepository.GetByIdAsync(reviewId);
    }

    public async Task AddAsync(Review review)
    {
        await reviewRepository.AddAsync(review);
    }

    public async Task UpdateAsync(Review review)
    {
        await reviewRepository.UpdateAsync(review);
    }

    public async Task DeleteAsync(int reviewId)
    {
        await reviewRepository.DeleteAsync(reviewId);
    }
}
