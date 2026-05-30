using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/gates")]
public class GatesController(IGateService gateService) : ControllerBase
{
    private const string EmptyGateNameErrorMessage = "The gate name cannot be empty.";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Gate>>> GetAll()
    {
        return this.Ok(await gateService.GetAllGatesAsync());
    }

    [HttpGet("{gateId:int}")]
    public async Task<ActionResult<Gate>> GetById(int gateId)
    {
        Gate? gate = await gateService.GetGateByIdAsync(gateId);

        if (gate == null)
        {
            return NotFound();
        }

        return this.Ok(gate);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] string gateName)
    {
        if (string.IsNullOrWhiteSpace(gateName))
        {
            return this.BadRequest(EmptyGateNameErrorMessage);
        }

        try
        {
            var gate = new Gate { GateName = gateName };
            await gateService.AddGateAsync(gate);
            return this.Ok(gate.Id);
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpPut("{gateId:int}")]
    public async Task<ActionResult> Update(int gateId, [FromBody] string updatedGateName)
    {
        if (await gateService.GetGateByIdAsync(gateId) == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(updatedGateName))
        {
            return this.BadRequest(EmptyGateNameErrorMessage);
        }

        try
        {
            var gate = new Gate { Id = gateId, GateName = updatedGateName };
            await gateService.UpdateGateAsync(gate);
            return this.NoContent();
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpDelete("{gateId:int}")]
    public async Task<ActionResult> Delete(int gateId)
    {
        if (await gateService.GetGateByIdAsync(gateId) == null)
        {
            return NotFound();
        }

        await gateService.DeleteGateAsync(gateId);
        return NoContent();
    }

    [HttpGet("{gateId:int}/has-flights")]
    public async Task<ActionResult<bool>> HasFlights(int gateId)
    {
        if (await gateService.GetGateByIdAsync(gateId) == null)
        {
            return NotFound();
        }

        return this.Ok(await gateService.HasFlightsAsync(gateId));
    }

    [HttpGet("{gateId:int}/delete-warning")]
    public async Task<ActionResult<string>> GetDeleteWarningMessage(int gateId)
    {
        if (await gateService.GetGateByIdAsync(gateId) == null)
        {
            return NotFound();
        }

        string deleteWarning = await gateService.GetDeleteWarningMessageAsync(gateId);

        return this.Ok(new { message = deleteWarning });
    }
}