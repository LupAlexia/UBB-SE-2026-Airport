using Microsoft.AspNetCore.Mvc;
using AirportLib.Domain.User;
using AirportApp.Web.Infrastructure;
using AirportApp.Web.Models.DutyFree;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Controllers;

public class DutyFreeController : Controller
{
    private readonly WebUserSession session;
    private readonly IShopService shopService;
    private readonly IManagerService managerService;

    public DutyFreeController(
        WebUserSession session,
        IShopService shopService,
        IManagerService managerService)
    {
        this.session = session;
        this.shopService = shopService;
        this.managerService = managerService;
    }

    public async Task<IActionResult> Index(string? search)
    {
        IEnumerable<Shop> shops;
        if (!string.IsNullOrWhiteSpace(search))
            shops = await shopService.SearchByNameAsync(search);
        else
            shops = await shopService.GetAllAvailableShopsAsync();

        var model = new ShopListViewModel
        {
            Shops = shops.ToList(),
            SearchQuery = search ?? string.Empty,
            UserRole = session.DutyFreeRole,
        };

        return session.IsDutyFreeManager
            ? View("AdminDashboard", model)
            : View(model);
    }

    [RequireDutyFreeRole(DutyFreeModuleRole.Client)]
    public async Task<IActionResult> Search(string? searchText, string? sort)
    {
        IEnumerable<Shop> shops;
        if (!string.IsNullOrWhiteSpace(searchText))
            shops = await shopService.SearchByNameAsync(searchText);
        else
            shops = await shopService.GetAllAvailableShopsAsync();

        if (sort == "name")
            shops = shops.OrderBy(s => s.Name);

        return PartialView("_ShopList", shops.ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireDutyFreeRole(DutyFreeModuleRole.Manager)]
    public async Task<IActionResult> AddShop(ShopFormModel form)
    {
        if (ModelState.IsValid)
        {
            var manager = await managerService.GetManagerByIdAsync(session.DutyFreeUserId);
            if (manager != null)
                await shopService.AddShopAsync(new Shop(form.Name, form.Type, manager));
        }
        return RedirectToAction(nameof(Index));
    }

    [RequireDutyFreeRole(DutyFreeModuleRole.Manager)]
    public async Task<IActionResult> EditShopForm(int id)
    {
        var shop = await shopService.GetShopByIdAsync(id);
        if (shop == null) return NotFound();

        var form = new ShopFormModel
        {
            Id = shop.Id,
            Name = shop.Name,
            Type = shop.Type,
            ManagerId = shop.Manager?.Id ?? 0
        };
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireDutyFreeRole(DutyFreeModuleRole.Manager)]
    public async Task<IActionResult> EditShop(ShopFormModel form)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Index));

        var existingShop = await shopService.GetShopByIdAsync(form.Id);
        if (existingShop == null) return NotFound();

        existingShop.Name = form.Name;
        existingShop.Type = form.Type;
        await shopService.UpdateShopAsync(existingShop);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireDutyFreeRole(DutyFreeModuleRole.Manager)]
    public async Task<IActionResult> DeleteShop(int id)
    {
        await shopService.DeleteShopAsync(id);
        return RedirectToAction(nameof(Index));
    }
}