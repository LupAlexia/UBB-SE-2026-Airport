using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IFAQRepository
{
    Task<IEnumerable<FAQEntry>> GetAsync();
    Task<FAQEntry?> GetByIdAsync(int faqEntryId);
    Task<IEnumerable<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category);
    Task<int> AddAsync(FAQEntry faqEntry);
    Task UpdateAsync(FAQEntry faqEntry);
    Task DeleteAsync(int faqEntryId);
    Task IncrementViewCountAsync(int faqEntryId);
    Task IncrementHelpfulCountAsync(int faqEntryId);
    Task IncrementNotHelpfulCountAsync(int faqEntryId);
}