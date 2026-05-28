using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IComplaintTicketService
{
    Task<IEnumerable<ComplaintTicket>> GetAllAsync();
    Task<ComplaintTicket?> GetByIdAsync(int complaintTicketId);
    Task AddAsync(ComplaintTicket complaintTicket);
    Task UpdateAsync(ComplaintTicket complaintTicket);
    Task DeleteAsync(int complaintTicketId);
    Task UpdateStatusAsync(int complaintTicketId, ComplaintTicketStatusEnum newStatus);
    Task UpdateUrgencyAsync(int complaintTicketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel);
    Task<IEnumerable<ComplaintTicketCategory>> GetAllCategoriesAsync();
    Task<ComplaintTicketCategory?> GetCategoryByIdAsync(int categoryId);
    Task<IEnumerable<ComplaintTicketSubcategory>> GetAllSubcategoriesAsync();
    Task<IEnumerable<ComplaintTicketSubcategory>> GetSubcategoriesByCategoryIdAsync(int categoryId);
    Task<ComplaintTicketSubcategory?> GetSubcategoryByIdAsync(int subcategoryId);
    Task<int> CountBySubcategoryNameAsync(string subcategoryName);
}
