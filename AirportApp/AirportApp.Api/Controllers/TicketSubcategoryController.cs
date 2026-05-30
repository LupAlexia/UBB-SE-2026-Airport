using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketSubcategoryController : ControllerBase
    {
        private readonly IComplaintTicketSubcategoryService ticketSubcategoryService;

        public TicketSubcategoryController(IComplaintTicketSubcategoryService ticketSubcategoryService)
        {
            this.ticketSubcategoryService = ticketSubcategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComplaintTicketSubcategory>>> GetAllAsync()
        {
            IEnumerable<ComplaintTicketSubcategory> subcategories = await ticketSubcategoryService.GetAllSubcategoriesAsync();
            return Ok(subcategories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintTicketSubcategory>> GetByIdAsync(int id)
        {
            try
            {
                ComplaintTicketSubcategory subcategory = await ticketSubcategoryService.GetSubcategoryByIdAsync(id);
                return Ok(subcategory);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ComplaintTicketSubcategory>>> GetByCategoryIdAsync(int categoryId)
        {
            IEnumerable<ComplaintTicketSubcategory> subcategories = await ticketSubcategoryService.GetSubcategoriesByCategoryIdAsync(categoryId);
            return Ok(subcategories);
        }
    }
}