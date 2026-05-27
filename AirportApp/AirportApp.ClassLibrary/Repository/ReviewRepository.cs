using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class ReviewRepository(AppDbContext databaseContext) : IReviewRepository
{
    private const string UserIdProperty = "UserId";

    public async Task<IEnumerable<Review>> GetAsync()
    {
        return await databaseContext.Reviews
            .Include(review => review.User)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Review?> GetByIdAsync(int reviewId)
    {
        var review = await databaseContext.Reviews
            .Include(review => review.User)
            .FirstOrDefaultAsync(review => review.Id == reviewId);

        if (review is null)
        {
            throw new KeyNotFoundException($"Review with id {reviewId} was not found.");
        }

        return review;
    }

    public async Task<int> AddAsync(Review review)
    {
        if (review is null)
        {
            throw new ArgumentNullException(nameof(review));
        }

        if (review.User is null || review.User.Id <= 0)
        {
            throw new ArgumentException("A review must be associated with a valid user.", nameof(review));
        }

        int userId = review.User.Id;
        review.User = null!;

        review.Id = 0;
        databaseContext.Reviews.Add(review);

        databaseContext.Entry(review)
            .Property(UserIdProperty)
            .CurrentValue = userId;

        await databaseContext.SaveChangesAsync();
        return review.Id;
    }

    public async Task UpdateAsync(Review review)
    {
        if (review is null)
        {
            throw new ArgumentNullException(nameof(review));
        }

        databaseContext.Reviews.Update(review);
        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int reviewId)
    {
        var reviewToRemove = await databaseContext.Reviews.FindAsync(reviewId);

        if (reviewToRemove is not null)
        {
            databaseContext.Reviews.Remove(reviewToRemove);
            await databaseContext.SaveChangesAsync();
        }
    }
}