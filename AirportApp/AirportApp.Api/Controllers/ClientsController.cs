using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController(IClientService clientService) : ControllerBase
{
    private const string MissingClientDataErrorMessage = "Client data cannot be null.";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Client>>> GetAll()
    {
        var clients = await clientService.GetAllClientsAsync();
        return this.Ok(clients);
    }

    [HttpGet("{clientId:int}")]
    public async Task<ActionResult<Client>> GetById(int clientId)
    {
        Client? client = await clientService.GetClientByIdAsync(clientId);

        if (client == null)
        {
            return this.NotFound();
        }

        return this.Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<Client>> Add([FromBody] Client client)
    {
        if (client == null)
        {
            return this.BadRequest(MissingClientDataErrorMessage);
        }

        await clientService.AddClientAsync(client);

        return this.CreatedAtAction(nameof(this.GetById), new { clientId = client.Id }, client);
    }

    [HttpPut("{clientId:int}")]
    public async Task<ActionResult<Client>> Update(int clientId, [FromBody] Client client)
    {

        if (client == null)
        {
            return this.BadRequest(MissingClientDataErrorMessage);
        }

        Client? existingClient = await clientService.GetClientByIdAsync(clientId);
        if (existingClient == null)
        {
            return this.NotFound();
        }

        client.Id = clientId;

        try
        {
            await clientService.UpdateClientAsync(client);
            return this.Ok(client);
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpDelete("{clientId:int}")]
    public async Task<IActionResult> Delete(int clientId)
    {
        Client? deletedClient = await clientService.GetClientByIdAsync(clientId);

        if (deletedClient == null)
        {
            return this.NotFound();
        }

        await clientService.DeleteClientAsync(clientId);
        return this.NoContent();
    }
}