using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IReviewService _reviewService;

    public DashboardController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }
    public IActionResult RedirectUser()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction(nameof(AdminDashboard));
        }
        if (User.IsInRole("Manager"))
        {
            return RedirectToAction(nameof(ManagerDashboard));
        }
        if (User.IsInRole("Employee"))
        {
            return RedirectToAction(nameof(EmployeeDashboard));
        }

        return RedirectToAction(nameof(CustomerSelection));
    }

    [Authorize(Roles = "Customer")]
    public IActionResult SupportDashboard()
    {
        return View();
    }

    [Authorize(Roles = "Customer")]
    public IActionResult CustomerSelection()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminDashboard()
    {
        var reviewsDtoList = new List<ReviewDTO>();

        try
        {
            var allReviews = (await _reviewService.GetAllAsync() ?? new List<Review>())
                .OrderByDescending(review => review.Id)
                .ToList();

            if (allReviews.Count > 0)
            {
                foreach (var review in allReviews)
                {
                    float reviewOverallAverage = await _reviewService.CalculateAverageRatingAsync(review);

                    reviewsDtoList.Add(new ReviewDTO(
                        review.Id,
                        review.User?.Id ?? 0,
                        review.User?.RetrieveConfiguredDisplayFullNameForBot() ?? "Anonymous Traveler",
                        review.Message,
                        review.DutyFreeRating,
                        review.FlightExperienceRating,
                        review.StaffFriendlinessRating,
                        review.CleanlinessRating,
                        reviewOverallAverage));
                }
            }
        }
        catch (Exception)
        {
            ViewBag.ErrorMessage = "Could not load client reviews at this time.";
        }

        return View(reviewsDtoList);
    }

    [Authorize(Roles = "Manager")]
    public IActionResult ManagerDashboard()
    {
        return RedirectToAction("Index", "CompanyDashboard");
    }

    [Authorize(Roles = "Employee")]
    public IActionResult EmployeeDashboard()
    {
        return RedirectToAction("Index", "EmployeeDashboard");
    }
}
