using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ComplaintTicketSubcategoryService(IComplaintTicketSubcategoryRepository subcategoryRepository) : IComplaintTicketSubcategoryService
{
    public async Task<IEnumerable<ComplaintTicketSubcategory>> GetAllSubcategoriesAsync()
    {
        return await subcategoryRepository.GetAsync();
    }

    public async Task<IEnumerable<ComplaintTicketSubcategory>> GetSubcategoriesByCategoryIdAsync(int categoryId)
    {
        return await subcategoryRepository.GetByCategoryIdAsync(categoryId);
    }

    public async Task<ComplaintTicketSubcategory> GetSubcategoryByIdAsync(int subcategoryId)
    {
        return await subcategoryRepository.GetByIdAsync(subcategoryId) ?? throw new KeyNotFoundException($"Subcategory {subcategoryId} not found.");
    }
}
