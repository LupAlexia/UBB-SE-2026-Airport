//using Microsoft.AspNetCore.Mvc;
//using AirportLib.Domain.User;
//using AirportApp.Web.Infrastructure;
//using AirportApp.ClassLibrary.Service.Interface;

//namespace AirportApp.Web.Controllers;

//public class DutyFreeItemDetailsController : Controller
//{
//    private readonly WebUserSession session;
//    private readonly IShopItemService shopItemService;
//    private readonly IShopService shopService;
//    private readonly ICartService cartService;

//    public DutyFreeItemDetailsController(
//        WebUserSession session,
//        IShopItemService shopItemService,
//        IShopService shopService,
//        ICartService cartService)
//    {
//        this.session = session;
//        this.shopItemService = shopItemService;
//        this.shopService = shopService;
//        this.cartService = cartService;
//    }

//    public IActionResult Index(int id)
//    {
//        var item = shopItemService.GetByIdAsync(id);
//        if (item == null)
//        {
//            return NotFound();
//        }

//        var shop = item.Shop ?? shopService.GetAllAvailableShopsAsync().FirstOrDefault(s => s.Id == item.Shop?.Id);
//        var cart = cartService.GetOrCreateCartAsync(session.DutyFreeUserId);
//        var cartItems = cartService.GetCartItemsAsync(cart.Id);
//        bool alreadyInCart = cartItems.Any(ci => ci.ShopItem?.Id == id);

//        var model = new ItemDetailsViewModel
//        {
//            Item = item,
//            Shop = shop!,
//            UserRole = session.DutyFreeRole,
//            CartId = cart.Id,
//            ItemAlreadyInCart = alreadyInCart,
//        };

//        return View(model);
//    }

//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    [RequireDutyFreeRole(DutyFreeModuleRole.Client)]
//    public IActionResult AddToCart(int itemId, int cartId, int quantity)
//    {
//        var item = shopItemService.GetByIdAsync(itemId);
//        if (item != null)
//        {
//            var cart = cartService.GetOrCreateCartAsync(session.DutyFreeUserId);
//            cartService.AddItemToCartAsync(cart.Id, new CartItem(0, item, quantity));
//        }

//        return RedirectToAction(nameof(Index), new { id = itemId });
//    }
//}

using Microsoft.AspNetCore.Mvc;
using AirportLib.Domain.User;
using AirportApp.Web.Infrastructure;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Web.Models.DutyFree; // Ensure correct path for ItemDetailsViewModel

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

    // Converted to async Task<IActionResult>
    public async Task<IActionResult> Index(int id)
    {
        // Added await to cleanly unpack the ShopItem
        var item = await shopItemService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        // Await the collection mapping fallback
        var availableShops = await shopService.GetAllAvailableShopsAsync();
        var shop = item.Shop ?? availableShops.FirstOrDefault(s => s.Id == item.Shop?.Id);

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

            // Adjusted parameters to match your ICartService implementation contract exactly
            await cartService.AddItemToCartAsync(cart.Id, item.Id, quantity);
        }

        return RedirectToAction(nameof(Index), new { id = itemId });
    }
}