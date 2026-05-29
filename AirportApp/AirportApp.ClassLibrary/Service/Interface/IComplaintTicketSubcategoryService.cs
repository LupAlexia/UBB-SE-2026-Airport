using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IComplaintTicketSubcategoryService
{
    Task<IEnumerable<ComplaintTicketSubcategory>> GetAllSubcategoriesAsync();
    Task<ComplaintTicketSubcategory> GetSubcategoryByIdAsync(int subcategoryId);
    Task<IEnumerable<ComplaintTicketSubcategory>> GetSubcategoriesByCategoryIdAsync(int categoryId);
}
