using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IComplaintTicketSubcategoryRepository
{
    Task<IEnumerable<ComplaintTicketSubcategory>> GetAsync();

    Task<ComplaintTicketSubcategory?> GetByIdAsync(int subcategoryId);

    Task<IEnumerable<ComplaintTicketSubcategory>> GetByCategoryIdAsync(int categoryId);
}