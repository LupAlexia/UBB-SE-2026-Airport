using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class ReviewServiceTests
{
    private const int    ValidReviewId = 1;
    private const string ValidMessage  = "Great experience!";

    private static User MakeUser(int id = 1) =>
        new User(id, "Test User", "test@test.com");

    private static Review MakeValidReview(int id = ValidReviewId) =>
        new Review(id, MakeUser(), ValidMessage, 4, 5, 3, 4);

    private static ReviewService MakeService(IEnumerable<Review>? existing = null)
    {
        var repo = Substitute.For<IReviewRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Review>>(existing ?? new List<Review>()));
        return new ReviewService(repo);
    }

    [Test]
    public void ValidateReviewAsync_NullReview_ThrowsArgumentNullException()
    {
        Assert.ThrowsAsync<ArgumentNullException>(
            () => MakeService().ValidateReviewAsync(null!));
    }

    [Test]
    public async Task ValidateReviewAsync_DuplicateReview_ThrowsArgumentException()
    {
        var existing = MakeValidReview();
        var service  = MakeService(new List<Review> { existing });

        Assert.ThrowsAsync<ArgumentException>(() => service.ValidateReviewAsync(existing));
    }

    [Test]
    public async Task ValidateReviewAsync_NullUser_ThrowsArgumentException()
    {
        var service = MakeService();
        var review  = MakeValidReview();
        review.User = null!;

        Assert.ThrowsAsync<ArgumentException>(() => service.ValidateReviewAsync(review));
    }

    [Test]
    public async Task ValidateReviewAsync_NullMessage_ThrowsArgumentException()
    {
        var service = MakeService();
        var review  = new Review
        {
            Id = ValidReviewId, User = MakeUser(), Message = null!,
            DutyFreeRating = 3, FlightExperienceRating = 3,
            StaffFriendlinessRating = 3, CleanlinessRating = 3
        };

        Assert.ThrowsAsync<ArgumentException>(() => service.ValidateReviewAsync(review));
    }

    [Test]
    public async Task ValidateReviewAsync_EmptyMessage_ThrowsArgumentException()
    {
        var service = MakeService();
        var review  = new Review
        {
            Id = ValidReviewId, User = MakeUser(), Message = string.Empty,
            DutyFreeRating = 3, FlightExperienceRating = 3,
            StaffFriendlinessRating = 3, CleanlinessRating = 3
        };

        Assert.ThrowsAsync<ArgumentException>(() => service.ValidateReviewAsync(review));
    }

    [Test]
    public async Task ValidateReviewAsync_DutyFreeRatingBelowMin_ThrowsArgumentException()
    {
        var service = MakeService();
        Assert.ThrowsAsync<ArgumentException>(() =>
            service.ValidateReviewAsync(new Review(ValidReviewId, MakeUser(), ValidMessage, 0, 3, 3, 3)));
    }

    [Test]
    public async Task ValidateReviewAsync_DutyFreeRatingAboveMax_ThrowsArgumentException()
    {
        var service = MakeService();
        Assert.ThrowsAsync<ArgumentException>(() =>
            service.ValidateReviewAsync(new Review(ValidReviewId, MakeUser(), ValidMessage, 6, 3, 3, 3)));
    }

    [Test]
    public async Task ValidateReviewAsync_FlightExperienceRatingBelowMin_ThrowsArgumentException()
    {
        var service = MakeService();
        Assert.ThrowsAsync<ArgumentException>(() =>
            service.ValidateReviewAsync(new Review(ValidReviewId, MakeUser(), ValidMessage, 3, 0, 3, 3)));
    }

    [Test]
    public async Task ValidateReviewAsync_FlightExperienceRatingAboveMax_ThrowsArgumentException()
    {
        var service = MakeService();
        Assert.ThrowsAsync<ArgumentException>(() =>
            service.ValidateReviewAsync(new Review(ValidReviewId, MakeUser(), ValidMessage, 3, 6, 3, 3)));
    }

    [Test]
    public async Task ValidateReviewAsync_StaffFriendlinessRatingBelowMin_ThrowsArgumentException()
    {
        var service = MakeService();
        Assert.ThrowsAsync<ArgumentException>(() =>
            service.ValidateReviewAsync(new Review(ValidReviewId, MakeUser(), ValidMessage, 3, 3, 0, 3)));
    }

    [Test]
    public async Task ValidateReviewAsync_StaffFriendlinessRatingAboveMax_ThrowsArgumentException()
    {
        var service = MakeService();
        Assert.ThrowsAsync<ArgumentException>(() =>
            service.ValidateReviewAsync(new Review(ValidReviewId, MakeUser(), ValidMessage, 3, 3, 6, 3)));
    }

    [Test]
    public async Task ValidateReviewAsync_CleanlinessRatingBelowMin_ThrowsArgumentException()
    {
        var service = MakeService();
        Assert.ThrowsAsync<ArgumentException>(() =>
            service.ValidateReviewAsync(new Review(ValidReviewId, MakeUser(), ValidMessage, 3, 3, 3, 0)));
    }

    [Test]
    public async Task ValidateReviewAsync_CleanlinessRatingAboveMax_ThrowsArgumentException()
    {
        var service = MakeService();
        Assert.ThrowsAsync<ArgumentException>(() =>
            service.ValidateReviewAsync(new Review(ValidReviewId, MakeUser(), ValidMessage, 3, 3, 3, 6)));
    }

    [TestCase(1, 1, 1, 1)]
    [TestCase(5, 5, 5, 5)]
    [TestCase(1, 5, 3, 2)]
    public async Task ValidateReviewAsync_RatingsAtValidBoundaries_DoesNotThrow(
        int duty, int flight, int staff, int clean)
    {
        var service = MakeService();
        Assert.DoesNotThrowAsync(() =>
            service.ValidateReviewAsync(
                new Review(ValidReviewId, MakeUser(), ValidMessage, duty, flight, staff, clean)));
    }

    [Test]
    public async Task ValidateReviewAsync_AllFieldsValid_DoesNotThrow()
    {
        var service = MakeService();
        Assert.DoesNotThrowAsync(() => service.ValidateReviewAsync(MakeValidReview()));
    }

    [Test]
    public void CreateReviewAsync_InvalidRating_ThrowsArgumentException()
    {
        var service = MakeService();

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateReviewAsync(ValidReviewId, MakeUser(), ValidMessage, 0, 3, 3, 3));
    }
    [Test]
    public async Task CalculateAverageRatingAsync_AllRatingsEqual_ReturnsExactAverage()
    {
        var service = MakeService();
        var review  = new Review(ValidReviewId, MakeUser(), ValidMessage, 4, 4, 4, 4);
        var result  = await service.CalculateAverageRatingAsync(review);
        Assert.That(result, Is.EqualTo(4.0f));
    }

    [Test]
    public async Task CalculateAverageRatingAsync_MixedRatings_ReturnsCorrectArithmeticMean()
    {
        var service = MakeService();
        var review  = new Review(ValidReviewId, MakeUser(), ValidMessage, 1, 2, 3, 4);
        var result  = await service.CalculateAverageRatingAsync(review);
        Assert.That(result, Is.EqualTo(2.5f));
    }

    [Test]
    public async Task CalculateAverageRatingAsync_AllMaxRatings_ReturnsFive()
    {
        var service = MakeService();
        var review  = new Review(ValidReviewId, MakeUser(), ValidMessage, 5, 5, 5, 5);
        var result  = await service.CalculateAverageRatingAsync(review);
        Assert.That(result, Is.EqualTo(5.0f));
    }

    [Test]
    public async Task CalculateAverageRatingAsync_AllMinRatings_ReturnsOne()
    {
        var service = MakeService();
        var review  = new Review(ValidReviewId, MakeUser(), ValidMessage, 1, 1, 1, 1);
        var result  = await service.CalculateAverageRatingAsync(review);
        Assert.That(result, Is.EqualTo(1.0f));
    }

    [Test]
    public async Task CalculateAverageRatingAsync_RatingsWithFractionalResult_ReturnsFractionalAverage()
    {
        var service = MakeService();
        var review  = new Review(ValidReviewId, MakeUser(), ValidMessage, 5, 5, 5, 4);
        var result  = await service.CalculateAverageRatingAsync(review);
        Assert.That(result, Is.EqualTo(4.75f));
    }
    [Test]
    public void GetByIdAsync_ReviewNotFound_ThrowsKeyNotFoundException()
    {
        var repo = Substitute.For<IReviewRepository>();
        repo.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Review?>(null));

        var service = new ReviewService(repo);

        Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByIdAsync(ValidReviewId));
    }

    [Test]
    public async Task GetByIdAsync_ReviewExists_ReturnsReview()
    {
        var review = MakeValidReview();
        var repo   = Substitute.For<IReviewRepository>();
        repo.GetByIdAsync(ValidReviewId).Returns(Task.FromResult<Review?>(review));

        var result = await new ReviewService(repo).GetByIdAsync(ValidReviewId);

        Assert.That(result, Is.SameAs(review));
    }
    [Test]
    public async Task GetAllAsync_RepositoryReturnsNull_ReturnsNull()
    {
        var repo = Substitute.For<IReviewRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Review>>(null!));

        var result = await new ReviewService(repo).GetAllAsync();

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_RepositoryHasData_ReturnsMaterialisedList()
    {
        var reviews = new List<Review> { MakeValidReview(1), MakeValidReview(2) };
        var repo    = Substitute.For<IReviewRepository>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Review>>(reviews));

        var result = await new ReviewService(repo).GetAllAsync();

        Assert.That(result!.Count, Is.EqualTo(2));
    }
}
