using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/runways")]
public class RunwaysController(IRunwayService runwayService) : ControllerBase
{
    private const string EmptyRunwayNameErrorMessage = "The runway name cannot be empty.";
    private const string InvalidHandleTimeErrorMessage = "Handle time must be a valid positive numeric value.";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Runway>>> GetAll()
    {
        return this.Ok(await runwayService.GetAllRunwaysAsync());
    }

    [HttpGet("{runwayId:int}")]
    public async Task<ActionResult<Runway>> GetById(int runwayId)
    {
        Runway? runway = await runwayService.GetRunwayByIdAsync(runwayId);

        if (runway == null)
        {
            return NotFound();
        }

        return this.Ok(runway);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] RunwayDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return this.BadRequest(EmptyRunwayNameErrorMessage);
        }

        if (dto.HandleTime <= 0)
        {
            return this.BadRequest(InvalidHandleTimeErrorMessage);
        }

        try
        {
            var runway = new Runway { Name = dto.Name, HandleTime = dto.HandleTime ?? 0 };
            await runwayService.AddRunwayAsync(runway);
            return this.Ok(runway.Id);
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpPut("{runwayId:int}")]
    public async Task<ActionResult> Update(int runwayId, [FromBody] RunwayDto dto)
    {
        if (await runwayService.GetRunwayByIdAsync(runwayId) == null)
        {
            return NotFound();
        }

        if (dto.Name != null && string.IsNullOrWhiteSpace(dto.Name))
        {
            return this.BadRequest(EmptyRunwayNameErrorMessage);
        }

        if (dto.HandleTime != null && dto.HandleTime <= 0)
        {
            return this.BadRequest(InvalidHandleTimeErrorMessage);
        }

        try
        {
            var runway = new Runway { Id = runwayId, Name = dto.Name!, HandleTime = dto.HandleTime ?? 0 };
            await runwayService.UpdateRunwayAsync(runway);
            return this.NoContent();
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpDelete("{runwayId:int}")]
    public async Task<ActionResult> Delete(int runwayId)
    {
        if (await runwayService.GetRunwayByIdAsync(runwayId) == null)
        {
            return NotFound();
        }

        try
        {
            await runwayService.DeleteRunwayAsync(runwayId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpGet("{runwayId:int}/has-flights")]
    public async Task<ActionResult<bool>> HasFlights(int runwayId)
    {
        if (await runwayService.GetRunwayByIdAsync(runwayId) == null)
        {
            return NotFound();
        }

        return this.Ok(await runwayService.HasFlightsAsync(runwayId));
    }

    [HttpGet("{runwayId:int}/delete-warning")]
    public async Task<ActionResult<string>> GetDeleteWarningMessage(int runwayId)
    {
        if (await runwayService.GetRunwayByIdAsync(runwayId) == null)
        {
            return NotFound();
        }

        string deleteWarning = await runwayService.GetDeleteWarningMessageAsync(runwayId);

        return this.Ok(new { message = deleteWarning });
    }

    public sealed class RunwayDto
    {
        public string? Name { get; set; }

        public int? HandleTime { get; set; }
    }
}