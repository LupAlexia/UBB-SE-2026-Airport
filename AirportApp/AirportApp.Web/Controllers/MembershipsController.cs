using System.Security.Claims;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Web.Controllers
{
    public class MembershipsController : Controller
    {
        private readonly IMembershipService membershipService;

        public MembershipsController(IMembershipService membershipService)
        {
            this.membershipService = membershipService;
        }

        private int? GetCurrentUserId()
        {
            if (User.Identity?.IsAuthenticated == true && User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int parsedId))
                {
                    return parsedId;
                }
            }
            return UserSession.CurrentUser?.Id;
        }

        public async Task<IActionResult> Index()
        {
            var memberships = await membershipService.GetAllMembershipsAsync();
            return View(memberships);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(int id)
        {
            int? userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "You must be logged in to purchase a membership.";
                return RedirectToAction("Index");
            }

            var result = await membershipService.PurchaseMembershipAsync(userId.Value, id);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index");
        }
    }
}

