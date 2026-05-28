using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ComplaintTicketService(
    IComplaintTicketRepository complaintTicketRepository,
    IComplaintTicketCategoryRepository categoryRepository,
    IComplaintTicketSubcategoryRepository subcategoryRepository) : IComplaintTicketService
{
    public async Task<IEnumerable<ComplaintTicket>> GetAllAsync()
    {
        return await complaintTicketRepository.GetAsync();
    }

    public async Task<ComplaintTicket?> GetByIdAsync(int complaintTicketId)
    {
        return await complaintTicketRepository.GetByIdAsync(complaintTicketId);
    }

    public async Task AddAsync(ComplaintTicket complaintTicket)
    {
        await complaintTicketRepository.AddAsync(complaintTicket);
    }

    public async Task UpdateAsync(ComplaintTicket complaintTicket)
    {
        await complaintTicketRepository.UpdateAsync(complaintTicket);
    }

    public async Task DeleteAsync(int complaintTicketId)
    {
        await complaintTicketRepository.DeleteAsync(complaintTicketId);
    }

    public async Task UpdateStatusAsync(int complaintTicketId, ComplaintTicketStatusEnum newStatus)
    {
        await complaintTicketRepository.UpdateStatusAsync(complaintTicketId, newStatus);
    }

    public async Task UpdateUrgencyAsync(int complaintTicketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
    {
        await complaintTicketRepository.UpdateUrgencyAsync(complaintTicketId, newUrgencyLevel);
    }

    public async Task<IEnumerable<ComplaintTicketCategory>> GetAllCategoriesAsync()
    {
        return await categoryRepository.GetAsync();
    }

    public async Task<ComplaintTicketCategory?> GetCategoryByIdAsync(int categoryId)
    {
        return await categoryRepository.GetByIdAsync(categoryId);
    }

    public async Task<IEnumerable<ComplaintTicketSubcategory>> GetAllSubcategoriesAsync()
    {
        return await subcategoryRepository.GetAsync();
    }

    public async Task<IEnumerable<ComplaintTicketSubcategory>> GetSubcategoriesByCategoryIdAsync(int categoryId)
    {
        return await subcategoryRepository.GetByCategoryIdAsync(categoryId);
    }

    public async Task<ComplaintTicketSubcategory?> GetSubcategoryByIdAsync(int subcategoryId)
    {
        return await subcategoryRepository.GetByIdAsync(subcategoryId);
    }

    public async Task<int> CountBySubcategoryNameAsync(string subcategoryName)
    {
        var all = await complaintTicketRepository.GetAsync();
        return all.Count(t => t.Subcategory?.SubcategoryName.Equals(subcategoryName, StringComparison.OrdinalIgnoreCase) ?? false);
    }
}
