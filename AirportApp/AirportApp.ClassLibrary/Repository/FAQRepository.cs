using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class FAQRepository(AppDbContext databaseContext) : IFAQRepository
{
    public async Task<IEnumerable<FAQEntry>> GetAsync()
    {
        return await databaseContext.Faqs
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<FAQEntry?> GetByIdAsync(int faqEntryId)
    {
        var faqEntry = await databaseContext.Faqs.FindAsync(faqEntryId);

        if (faqEntry is null)
        {
            throw new KeyNotFoundException($"FAQ with id {faqEntryId} was not found.");
        }

        return faqEntry;
    }

    public async Task<IEnumerable<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category)
    {
        if (category is FAQCategoryEnum.All)
        {
            return await GetAsync();
        }

        return await databaseContext.Faqs
            .Where(faqEntry => faqEntry.Category == category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> AddAsync(FAQEntry faqEntry)
    {
        if (faqEntry is null)
        {
            throw new ArgumentNullException(nameof(faqEntry));
        }

        faqEntry.Id = 0;
        databaseContext.Faqs.Add(faqEntry);
        await databaseContext.SaveChangesAsync();

        return faqEntry.Id;
    }

    public async Task UpdateAsync(FAQEntry faqEntry)
    {
        if (faqEntry is null)
        {
            throw new ArgumentNullException(nameof(faqEntry));
        }

        var existingEntry = await databaseContext.Faqs.FindAsync(faqEntry.Id);

        if (existingEntry is not null)
        {
            existingEntry.Question = faqEntry.Question;
            existingEntry.Answer = faqEntry.Answer;
            existingEntry.Category = faqEntry.Category;
            existingEntry.ViewCount = faqEntry.ViewCount;
            existingEntry.HelpfulVotesCount = faqEntry.HelpfulVotesCount;
            existingEntry.NotHelpfulVotesCount = faqEntry.NotHelpfulVotesCount;

            await databaseContext.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int faqEntryId)
    {
        var faqEntryToRemove = await databaseContext.Faqs.FindAsync(faqEntryId);

        if (faqEntryToRemove is not null)
        {
            databaseContext.Faqs.Remove(faqEntryToRemove);
            await databaseContext.SaveChangesAsync();
        }
    }

    public async Task IncrementViewCountAsync(int faqEntryId)
    {
        var faqEntry = await databaseContext.Faqs.FindAsync(faqEntryId);

        if (faqEntry is not null)
        {
            faqEntry.ViewCount++;
            await databaseContext.SaveChangesAsync();
        }
    }

    public async Task IncrementHelpfulCountAsync(int faqEntryId)
    {
        var faqEntry = await databaseContext.Faqs.FindAsync(faqEntryId);

        if (faqEntry is not null)
        {
            faqEntry.HelpfulVotesCount++;
            await databaseContext.SaveChangesAsync();
        }
    }

    public async Task IncrementNotHelpfulCountAsync(int faqEntryId)
    {
        var faqEntry = await databaseContext.Faqs.FindAsync(faqEntryId);

        if (faqEntry is not null)
        {
            faqEntry.NotHelpfulVotesCount++;
            await databaseContext.SaveChangesAsync();
        }
    }
}