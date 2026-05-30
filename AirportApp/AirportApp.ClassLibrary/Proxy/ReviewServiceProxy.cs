using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class ReviewServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IReviewService
{
    private const string BaseUrl = "api/review";

    public async Task<Review> GetByIdAsync(int identificationNumber)
    {
        return await GetRequiredAsync<Review>($"{BaseUrl}/{identificationNumber}");
    }

    public async Task<int> AddAsync(Review review)
    {
        return await PostForResultAsync<Review, int>(BaseUrl, review);
    }

    public async Task UpdateByIdAsync(int identificationNumber, Review review)
    {
        await PutAsync($"{BaseUrl}/{identificationNumber}", review);
    }

    public async Task DeleteByIdAsync(int identificationNumber)
    {
        await DeleteAsync($"{BaseUrl}/{identificationNumber}");
    }

    public async Task<List<Review>?> GetAllAsync()
    {
        return await GetListAsync<Review>(BaseUrl);
    }

    public Task CreateReviewAsync(int identificationNumber, User user, string message, int dutyFreeRating, int flightExperienceRating, int staffFriendlinessRating, int cleanlinessRating)
    {
        throw new NotSupportedException("CreateReviewAsync is not supported over HTTP.");
    }

    public Task ValidateReviewAsync(Review review)
    {
        throw new NotSupportedException("ValidateReviewAsync is not supported over HTTP.");
    }

    public Task<float> CalculateAverageRatingAsync(Review review)
    {
        return Task.FromResult((review.DutyFreeRating + review.FlightExperienceRating + review.StaffFriendlinessRating + review.CleanlinessRating) / 4.0f);
    }
}
