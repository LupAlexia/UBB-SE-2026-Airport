using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class FAQService(IFAQRepository faqRepository) : IFAQService
{
    public async Task<List<FAQEntry>> GetAllAsync()
    {
        return (await faqRepository.GetAsync()).ToList();
    }

    public async Task<List<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category)
    {
        return (await faqRepository.GetByCategoryAsync(category)).ToList();
    }

    public async Task AddFAQEntryAsync(FAQEntry newElem)
    {
        await faqRepository.AddAsync(newElem);
    }

    public async Task EditFAQEntryAsync(FAQEntry tempEntry, int faqEntryId)
    {
        await faqRepository.UpdateAsync(tempEntry);
    }

    public async Task DeleteFAQEntryAsync(int entryId)
    {
        await faqRepository.DeleteAsync(entryId);
    }

    public async Task IncrementViewCountAsync(FAQEntry entry)
    {
        await faqRepository.IncrementViewCountAsync(entry.Id);
    }

    public async Task IncrementWasHelpfulVotesAsync(FAQEntry entry)
    {
        await faqRepository.IncrementHelpfulCountAsync(entry.Id);
    }

    public async Task IncrementWasNotHelpfulVotesAsync(FAQEntry entry)
    {
        await faqRepository.IncrementNotHelpfulCountAsync(entry.Id);
    }

    public async Task<List<FAQEntry>> FilterFAQEntryAsync(FAQCategoryEnum category, string? searchQuery)
    {
        IEnumerable<FAQEntry> frequentlyAskedQuestions;

        if (category != FAQCategoryEnum.All)
            frequentlyAskedQuestions = await this.GetByCategoryAsync(category);
        else
            frequentlyAskedQuestions = await this.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            frequentlyAskedQuestions = frequentlyAskedQuestions.Where(question =>
                (question.Question?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (question.Answer?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        return frequentlyAskedQuestions.ToList();
    }
}
