using Microsoft.AspNetCore.Mvc;
using AirportLib.Domain.User;
using AirportApp.Web.Infrastructure;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.Web.Controllers;

[RequireDutyFreeRole(DutyFreeModuleRole.Client)]
public class DutyFreeCartController : Controller
{
    private readonly WebUserSession session;
    private readonly ICartService cartService;
    private readonly IReservationService reservationService;

    public DutyFreeCartController(
        WebUserSession session,
        ICartService cartService,
        IReservationService reservationService)
    {
        this.session = session;
        this.cartService = cartService;
        this.reservationService = reservationService;
    }

    public IActionResult Index()
    {
        var cart = cartService.GetOrCreateCart(session.DutyFreeUserId);
        var cartItems = cart.CartItems.ToList();
        var total = cart.GetOverallPrice();

        Reservation? activeReservation = null;
        try
        {
            activeReservation = reservationService.GetActiveReservationForCart(cart.Id);
        }
        catch
        {
            // No active reservation.
        }

        var itemViewModels = cartItems.Select(cartItem => new CartItemViewModel
        {
            CartItem = cartItem,
            IsLast = cartItem.Quantity == 1
        }).ToList();

        var model = new CartViewModel
        {
            CartId = cart.Id,
            Items = itemViewModels,
            Total = total,
            HasActiveReservation = activeReservation != null,
            ActiveReservationId = activeReservation?.Id,
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(int cartId, int cartItemId, int quantity)
    {
        if (!CanAccessCart(cartId))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        cartService.UpdateItemQuantity(cartId, cartItemId, quantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveItem(int cartId, int cartItemId)
    {
        if (!CanAccessCart(cartId))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        cartService.RemoveItemFromCart(cartId, cartItemId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Reserve(int cartId)
    {
        if (!CanAccessCart(cartId))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var cart = cartService.GetCartById(cartId);
        var reservation = new Reservation(cart, true, DateTime.UtcNow);
        reservationService.ReserveCart(reservation);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CancelReservation(int reservationId)
    {
        var reservation = reservationService.GetReservationById(reservationId);
        if (reservation.ReservationCart == null || !CanAccessCart(reservation.ReservationCart.Id))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        reservationService.CancelReservation(reservationId);
        return RedirectToAction(nameof(Index));
    }

    private bool CanAccessCart(int cartId)
    {
        return cartService.GetOrCreateCart(session.DutyFreeUserId).Id == cartId;
    }
}

