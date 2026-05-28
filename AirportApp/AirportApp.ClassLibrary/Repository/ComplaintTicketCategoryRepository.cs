using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class ComplaintTicketCategoryRepository(AppDbContext databaseContext) : IComplaintTicketCategoryRepository
{
    public async Task<IEnumerable<ComplaintTicketCategory>> GetAsync()
    {
        return await databaseContext.TicketCategories
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ComplaintTicketCategory?> GetByIdAsync(int categoryId)
    {
        return await databaseContext.TicketCategories.FindAsync(categoryId);
    }
}