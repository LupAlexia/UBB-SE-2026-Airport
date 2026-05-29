using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ComplaintTicketCategoryService(IComplaintTicketCategoryRepository categoryRepository) : IComplaintTicketCategoryService
{
    public async Task<ComplaintTicketCategory> GetCategoryByIdAsync(int categoryId)
    {
        return await categoryRepository.GetByIdAsync(categoryId) ?? throw new KeyNotFoundException($"Category {categoryId} not found.");
    }

    public async Task<IEnumerable<ComplaintTicketCategory>> GetAllCategoriesAsync()
    {
        return await categoryRepository.GetAsync();
    }
}
