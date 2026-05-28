using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IFAQService
{
    Task<IEnumerable<FAQEntry>> GetAllAsync();
    Task<FAQEntry?> GetByIdAsync(int faqEntryId);
    Task<IEnumerable<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category);
    Task AddAsync(FAQEntry faqEntry);
    Task UpdateAsync(FAQEntry faqEntry);
    Task DeleteAsync(int faqEntryId);
    Task IncrementViewCountAsync(int faqEntryId);
    Task IncrementHelpfulCountAsync(int faqEntryId);
    Task IncrementNotHelpfulCountAsync(int faqEntryId);
    Task<IEnumerable<FAQNode>> GetAllNodesAsync();
    Task<FAQNode?> GetNodeByIdAsync(int nodeId);
    Task AddNodeAsync(FAQNode faqNode);
    Task UpdateNodeAsync(FAQNode faqNode);
    Task DeleteNodeAsync(int nodeId);
}
