using AirportApp.Api.Controllers;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController(IReservationService reservationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reservation>>> GetAll()
    {
        return this.Ok(await reservationService.GetAllReservationsAsync());
    }

    [HttpGet("{reservationId:int}")]
    public async Task<ActionResult<Reservation>> GetById(int reservationId)
    {
        Reservation? reservation = await reservationService.GetReservationByIdAsync(reservationId);

        if (reservation == null)
        {
            return this.NotFound();
        }

        return this.Ok(reservation);
    }

    [HttpPost("reserve")]
    public async Task<ActionResult> Reserve([FromBody] ReserveCartRequest request)
    {
        if (request == null)
        {
            return this.BadRequest("Reservation data is required.");
        }

        Reservation reservation = new Reservation(
            new Cart(
                request.CartId,
                null!,
                request.CartItems
                    .Select(cartItemRequest => new CartItem(cartItemRequest.Id, new ShopItem { Id = cartItemRequest.ShopItemId }, cartItemRequest.Quantity))
                    .ToList()),
            request.Active,
            request.ReservationDate);

        try
        {
            await reservationService.ReserveCartAsync(reservation);
            return this.Ok(reservation.Id);
        }
        catch (InvalidOperationException conflictException)
        {
            return this.Conflict(conflictException.Message);
        }
    }

    [HttpDelete("{reservationId:int}")]
    public async Task<ActionResult> Delete(int reservationId)
    {
        await reservationService.DeleteReservationAsync(reservationId);
        return this.NoContent();
    }

    [HttpPut("{reservationId:int}/cancel")]
    public async Task<ActionResult> Cancel(int reservationId)
    {
        await reservationService.CancelReservationAsync(reservationId);
        return this.NoContent();
    }

    [HttpGet("cart/{cartId:int}/active")]
    public async Task<ActionResult<Reservation>> GetActiveReservationForCart(int cartId)
    {
        Reservation? reservation = await reservationService.GetActiveReservationForCartAsync(cartId);

        if (reservation == null)
        {
            return this.NotFound();
        }

        return this.Ok(reservation);
    }
}

public sealed record ReserveCartRequest(
    int CartId,
    bool Active,
    DateTime ReservationDate,
    List<CartItemRequest> CartItems);