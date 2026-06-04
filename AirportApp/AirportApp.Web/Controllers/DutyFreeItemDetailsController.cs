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
        var item = await shopItemService.GetByIdAsync(id);
        if (item == null)
            return NotFound();

        var shop = item.Shop ?? await shopService.GetShopByIdAsync(item.Shop?.Id ?? 0);
        var cart = await cartService.GetOrCreateCartAsync(session.DutyFreeUserId);
        var cartItems = await cartService.GetCartItemsAsync(cart.Id);
        bool alreadyInCart = cartItems.Any(ci => ci.ShopItem?.Id == id);

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
        var item = await shopItemService.GetByIdAsync(itemId);
        if (item != null)
        {
            var cart = await cartService.GetOrCreateCartAsync(session.DutyFreeUserId);
            await cartService.AddItemToCartAsync(cart.Id, itemId, quantity);
        }

        return RedirectToAction(nameof(Index), new { id = itemId });
    }
}