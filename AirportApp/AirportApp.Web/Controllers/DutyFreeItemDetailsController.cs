using Microsoft.AspNetCore.Mvc;
using AirportLib.Domain.User;
using AirportApp.Web.Infrastructure;
using AirportApp.ClassLibrary.Service.Interface;

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

    public IActionResult Index(int id)
    {
        var item = shopItemService.GetById(id);
        if (item == null)
        {
            return NotFound();
        }

        var shop = item.Shop ?? shopService.GetAllAvailableShops().FirstOrDefault(s => s.Id == item.Shop?.Id);
        var cart = cartService.GetOrCreateCart(session.DutyFreeUserId);
        var cartItems = cartService.GetCartItems(cart.Id);
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
    public IActionResult AddToCart(int itemId, int cartId, int quantity)
    {
        var item = shopItemService.GetById(itemId);
        if (item != null)
        {
            var cart = cartService.GetOrCreateCart(session.DutyFreeUserId);
            cartService.AddItemToCart(cart.Id, new CartItem(0, item, quantity));
        }

        return RedirectToAction(nameof(Index), new { id = itemId });
    }
}

