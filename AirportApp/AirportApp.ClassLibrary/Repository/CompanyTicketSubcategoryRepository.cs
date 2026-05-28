using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class ComplaintTicketSubcategoryRepository(AppDbContext databaseContext) : IComplaintTicketSubcategoryRepository
{
    public async Task<IEnumerable<ComplaintTicketSubcategory>> GetAsync()
    {
        return await databaseContext.TicketSubcategories
            .Include(subcategory => subcategory.ParentCategory)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ComplaintTicketSubcategory?> GetByIdAsync(int subcategoryId)
    {
        return await databaseContext.TicketSubcategories
            .Include(subcategory => subcategory.ParentCategory)
            .FirstOrDefaultAsync(subcategory => subcategory.Id == subcategoryId);
    }

    public async Task<IEnumerable<ComplaintTicketSubcategory>> GetByCategoryIdAsync(int categoryId)
    {
        return await databaseContext.TicketSubcategories
            .Include(subcategory => subcategory.ParentCategory)
            .Where(subcategory => subcategory.ParentCategory.Id == categoryId)
            .AsNoTracking()
            .ToListAsync();
    }
}