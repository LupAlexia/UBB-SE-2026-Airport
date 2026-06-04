using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
namespace AirportApp.Web.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly IReviewService reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        private static Customer? GetCurrentUser()
        {
            return UserSession.CurrentUser;
        }

        private static int? ResolveUserId(int? userId)
        {
            return userId ?? GetCurrentUser()?.Id;
        }

        private void PopulateCurrentUserIdViewBag()
        {
            var claimUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(claimUserId, out int parsedId))
            {
                ViewBag.UserId = parsedId;
            }
            else
            {
                ViewBag.UserId = UserSession.CurrentUser?.Id;
            }
        }

        // GET: Reviews
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            List<Review> reviews = (await reviewService.GetAllAsync() ?? new List<Review>())
                .OrderByDescending(review => review.Id)
                .ToList();

            var reviewDtos = new List<ReviewDTO>();
            double dutyFreeAverage = 0;
            double flightExperienceAverage = 0;
            double staffFriendlinessAverage = 0;
            double cleanlinessAverage = 0;
            double overallAverage = 0;

            if (reviews.Count > 0)
            {
                dutyFreeAverage = reviews.Average(review => review.DutyFreeRating);
                flightExperienceAverage = reviews.Average(review => review.FlightExperienceRating);
                staffFriendlinessAverage = reviews.Average(review => review.StaffFriendlinessRating);
                cleanlinessAverage = reviews.Average(review => review.CleanlinessRating);

                foreach (Review review in reviews)
                {
                    float reviewOverallAverage = await reviewService.CalculateAverageRatingAsync(review);
                    overallAverage += reviewOverallAverage;

                    reviewDtos.Add(new ReviewDTO(
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

                overallAverage /= reviews.Count;
            }

            var viewModel = new Models.Reviews.EmployeeReviewsIndexViewModel
            {
                TotalReviews = reviews.Count,
                AverageDutyFree = dutyFreeAverage,
                AverageFlightExperience = flightExperienceAverage,
                AverageStaffFriendliness = staffFriendlinessAverage,
                AverageCleanliness = cleanlinessAverage,
                AverageOverallRating = overallAverage,
                Reviews = reviewDtos
            };

            return View(viewModel);
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Review? review = await reviewService.GetByIdAsync(id.Value);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        [Authorize(Roles = "Employee,Customer")]
        public IActionResult Create(int? userId)
        {
            PopulateCurrentUserIdViewBag();

            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Employee,Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? userId, [Bind("Id,Message,DutyFreeRating,FlightExperienceRating,StaffFriendlinessRating,CleanlinessRating")] Review review)
        {
            int? resolvedUserId = ResolveUserId(userId);
            if (!resolvedUserId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "A user id is required to create a review.");
                PopulateCurrentUserIdViewBag();
                return View(review);
            }
            ModelState.Remove("User");
            ModelState.Remove("User.Id");

            if (ModelState.IsValid)
            {
                review.User = new User { Id = resolvedUserId.Value }; // proxy-ul face review.User.Id
                await reviewService.AddAsync(review);
                if (User.IsInRole("Employee"))
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Create));
            }

            ViewBag.UserId = resolvedUserId;
            return View(review);
        }

        // GET: Reviews/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await reviewService.GetByIdAsync(id.Value);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Message,DutyFreeRating,FlightExperienceRating,StaffFriendlinessRating,CleanlinessRating")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            ModelState.Remove("User");
            ModelState.Remove("User.Id");

            if (ModelState.IsValid)
            {
                var originalReview = await reviewService.GetByIdAsync(id);
                if (originalReview != null)
                {
                    review.User = originalReview.User;
                }
                await reviewService.UpdateByIdAsync(id, review);
                return RedirectToAction(nameof(Index));
            }
            return View(review);
        }

        // GET: Reviews/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await reviewService.GetByIdAsync(id.Value);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await reviewService.GetByIdAsync(id);
            if (review != null)
            {
                await reviewService.DeleteByIdAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return reviewService.GetByIdAsync(id) != null;
        }
    }
}

