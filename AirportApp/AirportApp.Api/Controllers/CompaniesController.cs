using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/companies")]
public class CompaniesController(ICompanyService companyService) : ControllerBase
{
    private const string EmptyCompanyNameErrorMessage = "The company name cannot be empty.";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Company>>> GetAll()
    {
        var companies = await companyService.GetAllCompaniesAsync();
        return this.Ok(companies);
    }

    [HttpGet("{companyId:int}")]
    public async Task<ActionResult<Company>> GetById(int companyId)
    {
        Company? company = await companyService.GetCompanyByIdAsync(companyId);

        if (company == null)
        {
            return this.NotFound();
        }

        return this.Ok(company);
    }

    [HttpPost]
    public async Task<ActionResult> Add([FromBody] string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            return this.BadRequest(EmptyCompanyNameErrorMessage);
        }

        var company = new Company { Name = companyName };
        await companyService.AddCompanyAsync(company);

        return this.Ok(company.Id);
    }

    [HttpPut("{companyId:int}")]
    public async Task<IActionResult> Update(int companyId, [FromBody] string updatedCompanyName)
    {
        if (await companyService.GetCompanyByIdAsync(companyId) == null)
        {
            return this.NotFound();
        }

        if (string.IsNullOrWhiteSpace(updatedCompanyName))
        {
            return this.BadRequest(EmptyCompanyNameErrorMessage);
        }

        var companyToUpdate = new Company { Id = companyId, Name = updatedCompanyName };
        await companyService.UpdateCompanyAsync(companyToUpdate);

        return this.NoContent();
    }

    [HttpDelete("{companyId:int}")]
    public async Task<IActionResult> Delete(int companyId)
    {
        if (await companyService.GetCompanyByIdAsync(companyId) == null)
        {
            return this.NotFound();
        }

        await companyService.DeleteCompanyAsync(companyId);
        return this.NoContent();
    }

    [HttpGet("{companyId:int}/flight-code")]
    public async Task<ActionResult<string>> GenerateFlightCode(int companyId)
    {
        string code = await companyService.GenerateFlightCodeUsingCompanyIdAsync(companyId);
        return this.Ok(code);
    }

    [HttpPost("validate-flight")]
    public async Task<ActionResult<int>> ValidateFlightInputs([FromBody] FlightValidationRequest request)
    {
        if (request == null)
        {
            return this.BadRequest();
        }

        int parsedCapacity = await companyService.ValidateFlightCreationInputsAsync(
            request.CompanyId,
            request.AirportId,
            request.CapacityText ?? string.Empty,
            request.RunwayId,
            request.GateId);

        return this.Ok(parsedCapacity);
    }

    public sealed class FlightValidationRequest
    {
        public int CompanyId { get; set; }

        public int AirportId { get; set; }

        public string? CapacityText { get; set; }

        public int RunwayId { get; set; }

        public int GateId { get; set; }
    }
}