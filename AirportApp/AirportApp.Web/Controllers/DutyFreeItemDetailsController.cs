using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Web.Infrastructure;
using AirportApp.Web.Models.DutyFree;
using AirportLib.Domain.User;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Web.Controllers;

public class DutyFreeItemDetailsController : Controller
{
    private readonly WebUserSession session;
    private readonly IShopItemService shopItemService;
    private readonly IShopService shopService;
    private readonly ICartService cartService;

    public DutyFreeItemDetailsController(
        WebUserSession session,
        IShopItemService shopItemService,
        IShopService shopService,
        ICartService cartService)
    {
        this.session = session;
        this.shopItemService = shopItemService;
        this.shopService = shopService;
        this.cartService = cartService;
    }

    public async Task<IActionResult> Index(int id)
    {
        if (id <= 0)
            return NotFound();

        var item = await shopItemService.GetByIdAsync(id);
        if (item == null)
            return NotFound();

        var shop = item.Shop ?? await shopService.GetShopByIdAsync(item.Shop?.Id ?? 0);
        // Ensure HttpContext session client id stays in sync with WebUserSession
        HttpContext?.Session?.SetInt32("ClientId", session.DutyFreeUserId);

        var cart = await cartService.GetOrCreateCartAsync(session.DutyFreeUserId);
        var cartItems = await cartService.GetCartItemsAsync(cart.Id);
        int existingQty = cartItems.Where(ci => ci.ShopItem?.Id == id).Sum(ci => ci.Quantity);
        // Consider item "already in cart" only when existing quantity is >= available stock
        bool alreadyInCart = existingQty >= item.Quantity;

        var model = new ItemDetailsViewModel
        {
            Item = item,
            Shop = shop!,
            UserRole = session.DutyFreeRole,
            CartId = cart.Id,
            ItemAlreadyInCart = alreadyInCart,
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireDutyFreeRole(DutyFreeModuleRole.Client)]
    public async Task<IActionResult> AddToCart(int itemId, int cartId, int quantity)
    {
        if (itemId <= 0 || quantity <= 0)
        {
            return BadRequest();
        }

        // keep session client id in sync
        HttpContext?.Session?.SetInt32("ClientId", session.DutyFreeUserId);

        var item = await shopItemService.GetByIdAsync(itemId);
        if (item != null)
        {
            var cart = await cartService.GetOrCreateCartAsync(session.DutyFreeUserId);
            await cartService.AddItemToCartAsync(cart.Id, itemId, quantity);
            TempData["CartSuccess"] = "Item added to cart.";
        }

        return RedirectToAction(nameof(Index), new { id = itemId });
    }
}