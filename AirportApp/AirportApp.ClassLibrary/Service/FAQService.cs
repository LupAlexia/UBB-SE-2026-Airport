using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class FAQService(IFAQRepository faqRepository, IDecisionTreeRepository decisionTreeRepository) : IFAQService
{
    public async Task<IEnumerable<FAQEntry>> GetAllAsync()
    {
        return await faqRepository.GetAsync();
    }

    public async Task<FAQEntry?> GetByIdAsync(int faqEntryId)
    {
        return await faqRepository.GetByIdAsync(faqEntryId);
    }

    public async Task<IEnumerable<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category)
    {
        return await faqRepository.GetByCategoryAsync(category);
    }

    public async Task AddAsync(FAQEntry faqEntry)
    {
        await faqRepository.AddAsync(faqEntry);
    }

    public async Task UpdateAsync(FAQEntry faqEntry)
    {
        await faqRepository.UpdateAsync(faqEntry);
    }

    public async Task DeleteAsync(int faqEntryId)
    {
        await faqRepository.DeleteAsync(faqEntryId);
    }

    public async Task IncrementViewCountAsync(int faqEntryId)
    {
        await faqRepository.IncrementViewCountAsync(faqEntryId);
    }

    public async Task IncrementHelpfulCountAsync(int faqEntryId)
    {
        await faqRepository.IncrementHelpfulCountAsync(faqEntryId);
    }

    public async Task IncrementNotHelpfulCountAsync(int faqEntryId)
    {
        await faqRepository.IncrementNotHelpfulCountAsync(faqEntryId);
    }

    public async Task<IEnumerable<FAQNode>> GetAllNodesAsync()
    {
        return await decisionTreeRepository.GetAsync();
    }

    public async Task<FAQNode?> GetNodeByIdAsync(int nodeId)
    {
        return await decisionTreeRepository.GetByIdAsync(nodeId);
    }

    public async Task AddNodeAsync(FAQNode faqNode)
    {
        await decisionTreeRepository.AddAsync(faqNode);
    }

    public async Task UpdateNodeAsync(FAQNode faqNode)
    {
        await decisionTreeRepository.UpdateAsync(faqNode);
    }

    public async Task DeleteNodeAsync(int nodeId)
    {
        await decisionTreeRepository.DeleteAsync(nodeId);
    }
}
