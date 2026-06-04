//using Microsoft.AspNetCore.Mvc;
//using AirportLib.Domain.User;
//using AirportApp.Web.Infrastructure;
//using AirportApp.ClassLibrary.Service.Interface;

//namespace AirportApp.Web.Controllers;

//[RequireDutyFreeRole(DutyFreeModuleRole.Client)]
//public class DutyFreeCartController : Controller
//{
//    private readonly WebUserSession session;
//    private readonly ICartService cartService;
//    private readonly IReservationService reservationService;

//    public DutyFreeCartController(
//        WebUserSession session,
//        ICartService cartService,
//        IReservationService reservationService)
//    {
//        this.session = session;
//        this.cartService = cartService;
//        this.reservationService = reservationService;
//    }

//    public IActionResult Index()
//    {
//        var cart = cartService.GetOrCreateCartAsync(session.DutyFreeUserId);
//        var cartItems = cart.CartItems.ToList();
//        var total = cart.GetOverallPrice();

//        Reservation? activeReservation = null;
//        try
//        {
//            activeReservation = reservationService.GetActiveReservationForCartAsync(cart.Id);
//        }
//        catch
//        {
//            // No active reservation.
//        }

//        var itemViewModels = cartItems.Select(cartItem => new CartItemViewModel
//        {
//            CartItem = cartItem,
//            IsLast = cartItem.Quantity == 1
//        }).ToList();

//        var model = new CartViewModel
//        {
//            CartId = cart.Id,
//            Items = itemViewModels,
//            Total = total,
//            HasActiveReservation = activeReservation != null,
//            ActiveReservationId = activeReservation?.Id,
//        };

//        return View(model);
//    }

//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public IActionResult UpdateQuantity(int cartId, int cartItemId, int quantity)
//    {
//        if (!CanAccessCart(cartId))
//        {
//            return StatusCode(StatusCodes.Status403Forbidden);
//        }

//        cartService.UpdateItemQuantityAsync(cartId, cartItemId, quantity);
//        return RedirectToAction(nameof(Index));
//    }

//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public IActionResult RemoveItem(int cartId, int cartItemId)
//    {
//        if (!CanAccessCart(cartId))
//        {
//            return StatusCode(StatusCodes.Status403Forbidden);
//        }

//        cartService.RemoveItemFromCartAsync(cartId, cartItemId);
//        return RedirectToAction(nameof(Index));
//    }

//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public IActionResult Reserve(int cartId)
//    {
//        if (!CanAccessCart(cartId))
//        {
//            return StatusCode(StatusCodes.Status403Forbidden);
//        }

//        var cart = cartService.GetCartByIdAsync(cartId);
//        var reservation = new Reservation(cart, true, DateTime.UtcNow);
//        reservationService.ReserveCartAsync(reservation);
//        return RedirectToAction(nameof(Index));
//    }

//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public IActionResult CancelReservation(int reservationId)
//    {
//        var reservation = reservationService.GetReservationByIdAsync(reservationId);
//        if (reservation.ReservationCart == null || !CanAccessCart(reservation.ReservationCart.Id))
//        {
//            return StatusCode(StatusCodes.Status403Forbidden);
//        }

//        reservationService.CancelReservationAsync(reservationId);
//        return RedirectToAction(nameof(Index));
//    }

//    private bool CanAccessCart(int cartId)
//    {
//        return cartService.GetOrCreateCartAsync(session.DutyFreeUserId).Id == cartId;
//    }
//}

using Microsoft.AspNetCore.Mvc;
using AirportLib.Domain.User;
using AirportApp.Web.Infrastructure;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Web.Models.DutyFree; // Ensure correct path for view models

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

    // Converted to async Task<IActionResult>
    public async Task<IActionResult> Index()
    {
        var cart = await cartService.GetOrCreateCartAsync(session.DutyFreeUserId);
        var cartItems = cart.CartItems.ToList();
        var total = cart.GetOverallPrice();

        Reservation? activeReservation = null;
        try
        {
            activeReservation = await reservationService.GetActiveReservationForCartAsync(cart.Id);
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
    public async Task<IActionResult> UpdateQuantity(int cartId, int cartItemId, int quantity)
    {
        if (!await CanAccessCart(cartId))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        await cartService.UpdateItemQuantityAsync(cartId, cartItemId, quantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveItem(int cartId, int cartItemId)
    {
        if (!await CanAccessCart(cartId))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        await cartService.RemoveItemFromCartAsync(cartId, cartItemId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(int cartId)
    {
        if (!await CanAccessCart(cartId))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var cart = await cartService.GetCartByIdAsync(cartId);
        if (cart == null)
        {
            return NotFound();
        }

        var reservation = new Reservation(cart, true, DateTime.UtcNow);
        await reservationService.ReserveCartAsync(reservation);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelReservation(int reservationId)
    {
        var reservation = await reservationService.GetReservationByIdAsync(reservationId);
        if (reservation == null || reservation.ReservationCart == null || !await CanAccessCart(reservation.ReservationCart.Id))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        await reservationService.CancelReservationAsync(reservationId);
        return RedirectToAction(nameof(Index));
    }

    // Helper method converted to return Task<bool>
    private async Task<bool> CanAccessCart(int cartId)
    {
        var userCart = await cartService.GetOrCreateCartAsync(session.DutyFreeUserId);
        return userCart.Id == cartId;
    }
}