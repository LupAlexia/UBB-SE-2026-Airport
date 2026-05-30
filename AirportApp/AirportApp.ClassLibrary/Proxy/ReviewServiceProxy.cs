using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class ReviewServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IReviewService
{
    private const string BaseUrl = "api/review";

    public async Task<Review> GetByIdAsync(int identificationNumber)
    {
        var dto = await GetRequiredAsync<ReviewDTO>($"{BaseUrl}/{identificationNumber}");
        return MapToEntity(dto);
    }

    public async Task<int> AddAsync(Review review)
    {
        var dto = new CreateReviewDTO(review.User.Id, review.Message, review.DutyFreeRating, review.FlightExperienceRating, review.StaffFriendlinessRating, review.CleanlinessRating);
        return await PostForResultAsync<CreateReviewDTO, int>(BaseUrl, dto);
    }

    public async Task UpdateByIdAsync(int identificationNumber, Review review)
    {
        var dto = new CreateReviewDTO(review.User.Id, review.Message, review.DutyFreeRating, review.FlightExperienceRating, review.StaffFriendlinessRating, review.CleanlinessRating);
        await PutAsync($"{BaseUrl}/{identificationNumber}", dto);
    }

    public async Task DeleteByIdAsync(int identificationNumber)
    {
        await DeleteAsync($"{BaseUrl}/{identificationNumber}");
    }

    public async Task<List<Review>?> GetAllAsync()
    {
        var dtos = await GetListAsync<ReviewDTO>(BaseUrl);
        return dtos.Select(MapToEntity).ToList();
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

    private static Review MapToEntity(ReviewDTO dto)
    {
        return new Review(dto.reviewId, new User(dto.userId, dto.userName, ""), dto.message, dto.dutyFreeRating, dto.flightExperienceRating, dto.staffFriendlinessRating, dto.cleanlinessRating);
    }
}
