using System.Threading.Tasks;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AirportApp.Web.Controllers
{
    public class DutyFreeCartController : Controller
    {
        private readonly ICartService _cartService;

        public DutyFreeCartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int shopItemId, int quantity = 1)
        {
            // Try to get current client id from session; fallback to 0 to create a cart for anonymous
            var clientId = HttpContext?.Session?.GetInt32("ClientId") ?? 0;

            var cart = await _cartService.GetOrCreateCartAsync(clientId);
            await _cartService.AddItemToCartAsync(cart.Id, shopItemId, quantity);

            // Redirect back to the shop items listing by default
            return RedirectToAction("Index", "DutyFreeShopItems");
        }
    }
}