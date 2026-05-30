using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministratorController : ControllerBase
    {
        private readonly IAdministratorService administratorService;

        public AdministratorController(IAdministratorService administratorService)
        {
            this.administratorService = administratorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Administrator>>> GetAllAsync()
        {
            IEnumerable<Administrator> administrators = await administratorService.GetAllAdministratorsAsync();
            return Ok(administrators);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Administrator>> GetByIdAsync(int id)
        {
            try
            {
                Administrator administrator = await administratorService.GetAdministratorByIdAsync(id);
                return Ok(administrator);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] Administrator administrator)
        {
            int createdId = await administratorService.AddAdministratorAsync(administrator);
            // i think i need this, but i am not sure if it is necessary since the service should return the created administrator with the ID populated
            administrator.Id = createdId;
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, administrator);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] Administrator administrator)
        {
            if (id != administrator.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            await administratorService.UpdateAdministratorByIdAsync(id, administrator);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await administratorService.DeleteAdministratorByIdAsync(id);
            return NoContent();
        }
    }
}