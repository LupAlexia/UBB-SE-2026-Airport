using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace AirportApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartsController(ICartService cartService) : ControllerBase
{
    private const string MissingCartDataErrorMessage = "Cart data cannot be null.";
    private const string MissingItemDataErrorMessage = "Cart item data cannot be null.";
    private const string MissingRequestDataErrorMessage = "Update request data cannot be null."; [HttpGet]
    public async Task<ActionResult<IEnumerable<Cart>>> GetAll()
    {
        // return this.Ok(cartService.GetAllCarts());
        var carts = await cartService.GetAllCartsAsync();
        return this.Ok(carts);
    }

    [HttpGet("{cartId:int}")]
    public async Task<ActionResult<Cart>> GetById(int cartId)
    {
        Cart? cart = await cartService.GetCartByIdAsync(cartId);

        if (cart == null)
        {
            return this.NotFound();
        }

        return this.Ok(cart);
    }

    [HttpPost]
    public async Task<ActionResult<Cart>> Add([FromBody] Cart cart)
    {
        if (cart == null)
        {
            return this.BadRequest(MissingCartDataErrorMessage);
        }

        await cartService.AddCartAsync(cart);

        return this.CreatedAtAction(nameof(this.GetById), new { cartId = cart.Id }, cart);
    }

    [HttpDelete("{cartId:int}")]
    public async Task<IActionResult> Delete(int cartId)
    {
        if (await cartService.GetCartByIdAsync(cartId) == null)
        {
            return this.NotFound();
        }

        await cartService.DeleteCartAsync(cartId);

        return this.NoContent();
    }

    [HttpPost("{cartId:int}/items")]
    public async Task<IActionResult> AddItemToCart(int cartId, [FromBody] CartItemRequest request)
    {
        if (request == null)
        {
            return this.BadRequest(MissingItemDataErrorMessage);
        }

        if (await cartService.GetCartByIdAsync(cartId) == null)
        {
            return this.NotFound();
        }

        try
        {
            await cartService.AddItemToCartAsync(cartId, request.ShopItemId, request.Quantity);
            return this.NoContent();
        }
        catch (System.InvalidOperationException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpDelete("{cartId:int}/items/{cartItemId:int}")]
    public async Task<IActionResult> RemoveItemFromCart(int cartId, int cartItemId)
    {
        if (await cartService.GetCartByIdAsync(cartId) == null)
        {
            return this.NotFound();
        }

        await cartService.RemoveItemFromCartAsync(cartId, cartItemId);

        return this.NoContent();
    }

    [HttpPut("{cartId:int}/items/{cartItemId:int}/quantity")]
    public async Task<IActionResult> UpdateItemQuantity(
        int cartId,
        int cartItemId,
        [FromBody] UpdateCartItemQuantityRequest request)
    {
        if (request == null)
        {
            return this.BadRequest(MissingRequestDataErrorMessage);
        }

        if (await cartService.GetCartByIdAsync(cartId) == null)
        {
            return this.NotFound();
        }

        await cartService.UpdateItemQuantityAsync(cartId, cartItemId, request.Quantity);

        return this.NoContent();
    }
    [HttpDelete("{cartId:int}/items")]
    public async Task<IActionResult> ClearCart(int cartId)
    {
        if (await cartService.GetCartByIdAsync(cartId) == null)
        {
            return this.NotFound();
        }

        await cartService.ClearCartAsync(cartId);

        return this.NoContent();
    }
}
public sealed record UpdateCartItemQuantityRequest(int Quantity);

public sealed record CartItemRequest(int Id, int ShopItemId, int Quantity);