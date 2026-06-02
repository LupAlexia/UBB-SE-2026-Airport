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

    public IActionResult Index(string? search)
    {
        IEnumerable<Shop> shops;
        if (!string.IsNullOrWhiteSpace(search))
        {
            shops = shopService.SearchByName(search);
        }
        else
        {
            shops = shopService.GetAllAvailableShops();
        }

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
    public IActionResult Search(string? searchText, string? sort)
    {
        IEnumerable<Shop> shops;

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            shops = shopService.SearchByName(searchText);
        }
        else
        {
            shops = shopService.GetAllAvailableShops();
        }

        if (sort == "name")
        {
            shops = shopService.SortAlphabetically(shops);
        }

        return PartialView("_ShopList", shops.ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireDutyFreeRole(DutyFreeModuleRole.Manager)]
    public IActionResult AddShop(ShopFormModel form)
    {
        if (ModelState.IsValid)
        {
            var manager = managerService.GetManagerById(session.DutyFreeUserId);
            shopService.AddShop(new Shop(form.Name, form.Type, manager!));
        }

        return RedirectToAction(nameof(Index));
    }

    [RequireDutyFreeRole(DutyFreeModuleRole.Manager)]
    public IActionResult EditShopForm(int id)
    {
        var shop = shopService.GetAllAvailableShops().FirstOrDefault(s => s.Id == id);
        if (shop == null)
        {
            return NotFound();
        }

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
    public IActionResult EditShop(ShopFormModel form)
    {
        if (!this.ModelState.IsValid)
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        Shop? existingShop = shopService.GetAllAvailableShops()
            .FirstOrDefault(shop => shop.Id == form.Id);

        if (existingShop == null)
        {
            return this.NotFound();
        }

        existingShop.Name = form.Name;
        existingShop.Type = form.Type;

        shopService.UpdateShop(existingShop);

        return this.RedirectToAction(nameof(this.Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireDutyFreeRole(DutyFreeModuleRole.Manager)]
    public IActionResult DeleteShop(int id)
    {
        shopService.DeleteShop(id);
        return RedirectToAction(nameof(Index));
    }
}

