using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShopsController : ControllerBase
{
    private readonly IShopService shopService;

    public ShopsController(IShopService shopService)
    {
        this.shopService = shopService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Shop>>> GetAllShops()
    {
        var shops = await shopService.GetAllAvailableShopsAsync();
        return Ok(shops);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Shop>> GetShopById(int id)
    {
        var shop = await shopService.GetShopByIdAsync(id);
        if (shop == null)
        {
            return NotFound();
        }

        return Ok(shop);
    }

    [HttpPost]
    public async Task<IActionResult> CreateShop([FromBody] ShopRequest shopRequest)
    {
        if (shopRequest == null)
        {
            return BadRequest("Shop data is null.");
        }

        try
        {
            Shop shop = ToShop(shopRequest);
            await shopService.AddShopAsync(shop);
            return CreatedAtAction(nameof(GetAllShops), new { id = shop.Id }, shop);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateShop(int id, [FromBody] ShopRequest shopRequest)
    {
        if (shopRequest == null)
        {
            return BadRequest("Shop data is null.");
        }

        try
        {
            Shop shop = ToShop(shopRequest);
            shop.Id = id;
            await shopService.UpdateShopAsync(shop);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShop(int id)
    {
        await shopService.DeleteShopAsync(id);
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Shop>>> SearchShops([FromQuery] string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return BadRequest("Search input cannot be empty.");
        }

        var results = await shopService.SearchByNameAsync(input);
        return Ok(results);
    }

    [HttpGet("sorted")]
    public async Task<ActionResult<IEnumerable<Shop>>> GetSortedShops()
    {
        var sortedShops = await shopService.SortAlphabeticallyAsync();
        return Ok(sortedShops);
    }

    private static Shop ToShop(ShopRequest request)
    {
        return new Shop(
            request.Id,
            request.Name,
            request.Type,
            new Manager(request.ManagerId, null, null, null));
    }

    public sealed record ShopRequest(int Id, string Name, string Type, int ManagerId);
}