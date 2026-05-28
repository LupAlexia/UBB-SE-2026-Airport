using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IComplaintTicketCategoryRepository
{
    Task<IEnumerable<ComplaintTicketCategory>> GetAsync();

    Task<ComplaintTicketCategory?> GetByIdAsync(int categoryId);
}