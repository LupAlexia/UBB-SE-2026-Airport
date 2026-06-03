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

    public async Task<IActionResult> Index(int shopId, string? search, string? sort)
    {
        var shop = await shopService.GetShopByIdAsync(shopId);
        if (shop == null)
            return NotFound();

        IEnumerable<ShopItem> items;
        if (!string.IsNullOrWhiteSpace(search))
            items = await shopItemService.SearchItemsByNameAsync(shopId, search);
        else if (sort == "price")
            items = await shopItemService.GetItemsSortedByPriceAsync(shop);
        else if (sort == "name")
            items = await shopItemService.GetItemsSortedAlphabeticallyAsync(shop);
        else
            items = await shopItemService.GetItemsByShopIdAsync(shopId);

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
    public async Task<IActionResult> AddItem(ShopItemFormModel form)
    {
        if (ModelState.IsValid)
        {
            var shop = await shopService.GetShopByIdAsync(form.ShopId);
            if (shop != null)
                await shopItemService.AddShopItemAsync(new ShopItem(form.Quantity, form.Price, shop, string.Empty, form.Name, string.Empty));
        }
        return RedirectToAction(nameof(Index), new { shopId = form.ShopId });
    }

    [RequireDutyFreeRole(DutyFreeModuleRole.Manager)]
    public async Task<IActionResult> EditItemForm(int id)
    {
        var item = await shopItemService.GetByIdAsync(id);
        if (item == null)
            return NotFound();

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
    public async Task<IActionResult> EditItem(ShopItemFormModel form)
    {
        if (ModelState.IsValid)
        {
            var existing = await shopItemService.GetByIdAsync(form.Id);
            if (existing != null)
            {
                existing.Name = form.Name;
                existing.Price = form.Price;
                existing.Quantity = form.Quantity;
                await shopItemService.UpdateShopItemAsync(existing);
            }
        }
        return RedirectToAction(nameof(Index), new { shopId = form.ShopId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireDutyFreeRole(DutyFreeModuleRole.Manager)]
    public async Task<IActionResult> DeleteItem(int id, int shopId)
    {
        await shopItemService.RemoveShopItemAsync(id);
        return RedirectToAction(nameof(Index), new { shopId });
    }
}