using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/managers")]
public class ManagersController(IManagerService managerService) : ControllerBase
{
    private const string MissingManagerDataErrorMessage = "Manager data cannot be null.";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Manager>>> GetAll()
    {
        return this.Ok(await managerService.GetAllManagersAsync());
    }

    [HttpGet("{managerId:int}")]
    public async Task<ActionResult<Manager>> GetById(int managerId)
    {
        Manager? manager = await managerService.GetManagerByIdAsync(managerId);

        if (manager == null)
        {
            return this.NotFound();
        }

        return this.Ok(manager);
    }

    [HttpPost]
    public async Task<ActionResult<Manager>> Add([FromBody] Manager manager)
    {
        if (manager == null)
        {
            return this.BadRequest(MissingManagerDataErrorMessage);
        }

        try
        {
            await managerService.AddManagerAsync(manager);
            return this.CreatedAtAction(nameof(this.GetById), new { managerId = manager.Id }, manager);
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpPut("{managerId:int}")]
    public async Task<ActionResult<Manager>> Update(int managerId, [FromBody] Manager manager)
    {
        if (manager == null)
        {
            return this.BadRequest(MissingManagerDataErrorMessage);
        }

        if (await managerService.GetManagerByIdAsync(managerId) == null)
        {
            return this.NotFound();
        }

        manager.Id = managerId;

        try
        {
            await managerService.UpdateManagerAsync(manager);
            return this.Ok(manager);
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpDelete("{managerId:int}")]
    public async Task<IActionResult> Delete(int managerId)
    {
        if (await managerService.GetManagerByIdAsync(managerId) == null)
        {
            return this.NotFound();
        }

        await managerService.DeleteManagerAsync(managerId);
        return this.NoContent();
    }
}