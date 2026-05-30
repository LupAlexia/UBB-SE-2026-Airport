using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/shop-items")]
public class ShopItemsController(IShopItemService shopItemService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShopItem>>> GetAll()
    {
        return this.Ok(await shopItemService.GetAllAsync());
    }

    [HttpGet("{shopItemId:int}")]
    public async Task<ActionResult<ShopItem>> GetById(int shopItemId)
    {
        ShopItem shopItem = await shopItemService.GetByIdAsync(shopItemId);
        return this.Ok(shopItem);
    }

    [HttpGet("shop/{shopId:int}/")]
    public async Task<ActionResult<IEnumerable<ShopItem>>> GetByShopId(int shopId)
    {
        return this.Ok(await shopItemService.GetItemsByShopIdAsync(shopId));
    }

    [HttpGet("shop/{shopId:int}/search")]
    public async Task<ActionResult<IEnumerable<ShopItem>>> Search(int shopId, [FromQuery] string searchText = "")
    {
        return this.Ok(await shopItemService.SearchItemsByNameAsync(shopId, searchText));
    }

    [HttpGet("shop/{shopId:int}/sorted-by-price")]
    public async Task<ActionResult<IEnumerable<ShopItem>>> GetSortedByPrice(int shopId)
    {
        Shop shop = new Shop(shopId, null!, null!, null!);

        return this.Ok(await shopItemService.GetItemsSortedByPriceAsync(shop));
    }

    [HttpGet("shop/{shopId:int}/sorted-alphabetically")]
    public async Task<ActionResult<IEnumerable<ShopItem>>> GetSortedAlphabetically(int shopId)
    {
        Shop shop = new Shop(shopId, null!, null!, null!);

        return this.Ok(await shopItemService.GetItemsSortedAlphabeticallyAsync(shop));
    }

    [HttpPost]
    public async Task<ActionResult> Add([FromBody] ShopItemRequest shopItemRequest)
    {
         await shopItemService.AddShopItemAsync(ToShopItem(shopItemRequest));
         return this.Ok();
        
    }

    [HttpPut("{shopItemId:int}")]
    public async Task<ActionResult> Update(int shopItemId, [FromBody] ShopItemRequest shopItemRequest)
    {
        ShopItem shopItem = ToShopItem(shopItemRequest);
        shopItem.Id = shopItemId;

        await shopItemService.UpdateShopItemAsync(shopItem);
        return this.NoContent();
    }

    [HttpDelete("{shopItemId:int}")]
    public async Task<ActionResult> Delete(int shopItemId)
    {
        await shopItemService.RemoveShopItemAsync(shopItemId);
        return NoContent();
    }

    private static ShopItem ToShopItem(ShopItemRequest request)
    {
        return new ShopItem
        {
            Id = request.Id,
            Quantity = request.Quantity,
            Price = request.Price,
            Shop = new Shop(request.ShopId, null!, null!, null!),
            Photo = request.Photo,
            Name = request.Name,
            Description = request.Description
        };
    }

    public sealed record ShopItemRequest(
        int Id,
        int Quantity,
        float Price,
        int ShopId,
        string Photo,
        string Name,
        string Description);
}