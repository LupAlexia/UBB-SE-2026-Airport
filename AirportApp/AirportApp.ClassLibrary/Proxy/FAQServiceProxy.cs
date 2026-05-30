using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class FAQServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IFAQService
{
    private const string BaseUrl = "api/faq";

    public async Task<List<FAQEntry>> GetAllAsync()
    {
        return await GetListAsync<FAQEntry>(BaseUrl);
    }

    public async Task<List<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category)
    {
        return await GetListAsync<FAQEntry>($"{BaseUrl}/category/{category}");
    }

    public async Task AddFAQEntryAsync(FAQEntry newElem)
    {
        await PostAsync(BaseUrl, newElem);
    }

    public async Task EditFAQEntryAsync(FAQEntry tempEntry, int faqEntryId)
    {
        await PutAsync($"{BaseUrl}/{faqEntryId}", tempEntry);
    }

    public async Task DeleteFAQEntryAsync(int entryId)
    {
        await DeleteAsync($"{BaseUrl}/{entryId}");
    }

    public async Task IncrementViewCountAsync(FAQEntry entry)
    {
        await PutAsync<object>($"{BaseUrl}/{entry.Id}/increment-view", null!);
    }

    public async Task IncrementWasHelpfulVotesAsync(FAQEntry entry)
    {
        await PutAsync<object>($"{BaseUrl}/{entry.Id}/increment-helpful", null!);
    }

    public async Task IncrementWasNotHelpfulVotesAsync(FAQEntry entry)
    {
        await PutAsync<object>($"{BaseUrl}/{entry.Id}/increment-nothelpful", null!);
    }

    public async Task<List<FAQEntry>> FilterFAQEntryAsync(FAQCategoryEnum category, string? searchQuery)
    {
        return await GetListAsync<FAQEntry>($"{BaseUrl}/filter?category={category}&searchQuery={Uri.EscapeDataString(searchQuery ?? "")}");
    }
}
