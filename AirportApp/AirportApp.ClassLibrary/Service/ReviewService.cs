using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ReviewService(IReviewRepository reviewRepository) : IReviewService
{
    private const int MinRating = 1;
    private const int MaxRating = 5;
    private const int NumberOfRatings = 4;

    public async Task<Review> GetByIdAsync(int identificationNumber)
    {
        return await reviewRepository.GetByIdAsync(identificationNumber) ?? throw new KeyNotFoundException($"Review {identificationNumber} not found.");
    }

    public async Task<int> AddAsync(Review review)
    {
        return await reviewRepository.AddAsync(review);
    }

    public async Task UpdateByIdAsync(int identificationNumber, Review review)
    {
        await reviewRepository.UpdateAsync(review);
    }

    public async Task DeleteByIdAsync(int identificationNumber)
    {
        await reviewRepository.DeleteAsync(identificationNumber);
    }

    public async Task<List<Review>?> GetAllAsync()
    {
        var reviews = await reviewRepository.GetAsync();
        return reviews?.ToList();
    }

    public async Task CreateReviewAsync(int identificationNumber, User user, string message, int dutyFreeRating, int flightExperienceRating, int staffFriendlinessRating, int cleanlinessRating)
    {
        Review review = new(identificationNumber, user, message, dutyFreeRating, flightExperienceRating, staffFriendlinessRating, cleanlinessRating);
        await ValidateReviewAsync(review);
        await AddAsync(review);
    }

    public async Task ValidateReviewAsync(Review review)
    {
        ArgumentNullException.ThrowIfNull(review);

        var allReviews = await this.GetAllAsync();
        if (allReviews != null && allReviews.Contains(review))
            throw new ArgumentException("Review already exists");

        if (review.User == null)
            throw new ArgumentException("User cannot be null");

        if (string.IsNullOrEmpty(review.Message))
            throw new ArgumentException("Message cannot be null or empty");

        if (review.DutyFreeRating < MinRating || review.DutyFreeRating > MaxRating)
            throw new ArgumentException($"Duty Free Rating must be between {MinRating} and {MaxRating}");

        if (review.FlightExperienceRating < MinRating || review.FlightExperienceRating > MaxRating)
            throw new ArgumentException($"Flight Experience Rating must be between {MinRating} and {MaxRating}");

        if (review.StaffFriendlinessRating < MinRating || review.StaffFriendlinessRating > MaxRating)
            throw new ArgumentException($"Staff Friendliness Rating must be between {MinRating} and {MaxRating}");

        if (review.CleanlinessRating < MinRating || review.CleanlinessRating > MaxRating)
            throw new ArgumentException($"Cleanliness Rating must be between {MinRating} and {MaxRating}");
    }

    public Task<float> CalculateAverageRatingAsync(Review review)
    {
        float average = (review.DutyFreeRating +
                         review.FlightExperienceRating +
                         review.StaffFriendlinessRating +
                         review.CleanlinessRating) / (float)NumberOfRatings;
        return Task.FromResult(average);
    }
}
