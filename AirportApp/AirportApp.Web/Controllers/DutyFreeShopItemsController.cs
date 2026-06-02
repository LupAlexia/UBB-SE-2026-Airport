using Microsoft.AspNetCore.Mvc;
using AirportLib.Domain.User;
using AirportApp.Web.Infrastructure;
using AirportApp.Web.Models.DutyFree;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Controllers;

public class DutyFreeShopItemsController : Controller
{
    private readonly WebUserSession session;
    private readonly IShopService shopService;
    private readonly IShopItemService shopItemService;

    public DutyFreeShopItemsController(
        WebUserSession session,
        IShopService shopService,
        IShopItemService shopItemService)
    {
        this.session = session;
        this.shopService = shopService;
        this.shopItemService = shopItemService;
    }

    public IActionResult Index(int shopId, string? search, string? sort)
    {
        var shop = shopService.GetShopById(shopId);
        if (shop == null)
        {
            return NotFound();
        }

        IEnumerable<ShopItem> items;
        if (!string.IsNullOrWhiteSpace(search))
        {
            items = shopItemService.SearchItemsByName(shopId, search);
        }
        else if (sort == "price")
        {
            items = shopItemService.GetItemsSortedByPrice(shop);
        }
        else if (sort == "name")
        {
            items = shopItemService.GetItemsSortedAlphabetically(shop);
        }
        else
        {
            items = shopItemService.GetItemsByShopId(shopId);
        }

        var model = new ShopItemsViewModel
        {
            Shop = shop,
            Items = items.ToList(),
            SearchQuery = search ?? string.Empty,
            SortOrder = sort ?? "default",
            UserRole = session.DutyFreeRole,
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireDutyFreeRole(DutyFreeModuleRole.Manager)]
    public IActionResult AddItem(ShopItemFormModel form)
    {
        if (ModelState.IsValid)
        {
            var shop = shopService.GetShopById(form.ShopId);
            if (shop != null)
            {
                shopItemService.AddShopItem(new ShopItem(form.Quantity, form.Price, shop, string.Empty, form.Name, string.Empty));
            }
        }

        return RedirectToAction(nameof(Index), new { shopId = form.ShopId });
    }

    [RequireDutyFreeRole(DutyFreeModuleRole.Manager)]
    public IActionResult EditItemForm(int id)
    {
        var item = shopItemService.GetById(id);
        if (item == null)
        {
            return NotFound();
        }

        var form = new ShopItemFormModel
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            Quantity = item.Quantity,
            ShopId = item.Shop?.Id ?? 0,
        };

        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireDutyFreeRole(DutyFreeModuleRole.Manager)]
    public IActionResult EditItem(ShopItemFormModel form)
    {
        if (ModelState.IsValid)
        {
            var existing = shopItemService.GetById(form.Id);
            if (existing != null)
            {
                existing.Name = form.Name;
                existing.Price = form.Price;
                existing.Quantity = form.Quantity;
                shopItemService.UpdateShopItem(existing);
            }
        }

        return RedirectToAction(nameof(Index), new { shopId = form.ShopId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireDutyFreeRole(DutyFreeModuleRole.Manager)]
    public IActionResult DeleteItem(int id, int shopId)
    {
        shopItemService.RemoveShopItem(id);
        return RedirectToAction(nameof(Index), new { shopId });
    }
}

