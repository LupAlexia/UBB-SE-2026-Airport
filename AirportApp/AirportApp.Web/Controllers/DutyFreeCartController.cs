using System.Threading.Tasks;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AirportApp.Web.Controllers
{
    public class DutyFreeCartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IShopItemService _shopItemService;
        private readonly IReservationService _reservationService;
        private readonly WebUserSession _session;

        public DutyFreeCartController(
            ICartService cartService,
            IShopItemService shopItemService,
            IReservationService reservationService,
            WebUserSession session)
        {
            _cartService = cartService;
            _shopItemService = shopItemService;
            _reservationService = reservationService;
            _session = session;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int cartId)
        {
            if (cartId <= 0)
            {
                return BadRequest();
            }

            var cart = await _cartService.GetCartByIdAsync(cartId);
            if (cart == null)
            {
                TempData["CartError"] = "No cart to reserve.";
                return RedirectToAction(nameof(Index));
            }

            var reservation = new AirportApp.ClassLibrary.Entity.Domain.Reservation(cart, true, DateTime.Now);
            try
            {
                await _reservationService.ReserveCartAsync(reservation);
                TempData["CartSuccess"] = "Cart reserved successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["CartError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            if (reservationId <= 0)
                return BadRequest();

            try
            {
                await _reservationService.CancelReservationAsync(reservationId);
                TempData["CartSuccess"] = "Reservation cancelled.";
            }
            catch (Exception ex)
            {
                TempData["CartError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index()
        {
            var clientId = _session.DutyFreeUserId;

            var cart = await _cartService.GetOrCreateCartAsync(clientId);
            if (cart == null)
            {
                // No cart available - show empty cart
                return View(new AirportApp.Web.Models.DutyFree.CartViewModel());
            }

            var cartItems = (await _cartService.GetCartItemsAsync(cart.Id)).ToList();

            var model = new AirportApp.Web.Models.DutyFree.CartViewModel
            {
                CartId = cart.Id,
                Items = cartItems.Select(ci => new AirportApp.Web.Models.DutyFree.CartItemViewModel
                {
                    CartItem = ci,
                    IsLast = ci.Quantity == 1
                }).ToList(),
                Total = await _cartService.GetCartTotalAsync(cart.Id)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int shopItemId, int quantity = 1)
        {
            var clientId = _session.DutyFreeUserId;

            if (shopItemId <= 0 || quantity <= 0)
            {
                return BadRequest();
            }

            var item = await _shopItemService.GetByIdAsync(shopItemId);
            if (item == null)
            {
                return NotFound();
            }

            if (item.Quantity <= 0)
            {
                TempData["CartError"] = "Item is out of stock.";
                return RedirectToAction("Index", "DutyFreeItemDetails", new { id = shopItemId });
            }

            var cart = await _cartService.GetOrCreateCartAsync(clientId);
            try
            {
                await _cartService.AddItemToCartAsync(cart.Id, shopItemId, quantity);
                TempData["CartSuccess"] = "Item added to cart.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["CartError"] = ex.Message;
                return RedirectToAction("Index", "DutyFreeItemDetails", new { id = shopItemId });
            }

            return RedirectToAction("Index", "DutyFreeCart");
        }
    }
}