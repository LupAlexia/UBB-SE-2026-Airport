using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class FAQServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IFAQService
{
    private const string BaseUrl = "api/faq";

    public async Task<List<FAQEntry>> GetAllAsync()
    {
        var dtos = await GetListAsync<FAQEntryDTO>(BaseUrl);
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<List<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category)
    {
        var dtos = await GetListAsync<FAQEntryDTO>($"{BaseUrl}/by-category?category={category}");
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task AddFAQEntryAsync(FAQEntry newElem)
    {
        await PostAsync(BaseUrl, MapToDto(newElem));
    }

    public async Task EditFAQEntryAsync(FAQEntry tempEntry, int faqEntryId)
    {
        await PutAsync($"{BaseUrl}/{faqEntryId}", MapToDto(tempEntry));
    }

    public async Task DeleteFAQEntryAsync(int entryId)
    {
        await DeleteAsync($"{BaseUrl}/{entryId}");
    }

    public async Task IncrementViewCountAsync(FAQEntry entry)
    {
        await PostAsync<object>($"{BaseUrl}/{entry.Id}/increment-view", null!);
    }

    public async Task IncrementWasHelpfulVotesAsync(FAQEntry entry)
    {
        await PostAsync<object>($"{BaseUrl}/{entry.Id}/increment-helpful", null!);
    }

    public async Task IncrementWasNotHelpfulVotesAsync(FAQEntry entry)
    {
        await PostAsync<object>($"{BaseUrl}/{entry.Id}/increment-not-helpful", null!);
    }

    public async Task<List<FAQEntry>> FilterFAQEntryAsync(FAQCategoryEnum category, string? searchQuery)
    {
        var dtos = await GetListAsync<FAQEntryDTO>($"{BaseUrl}/filter?category={category}&searchQuery={Uri.EscapeDataString(searchQuery ?? "")}");
        return dtos.Select(MapToEntity).ToList();
    }

    private static FAQEntry MapToEntity(FAQEntryDTO dto)
    {
        return new FAQEntry(dto.Id, dto.Question, dto.Answer, dto.Category, dto.ViewCount, dto.HelpfulVotesCount, dto.NotHelpfulVotesCount);
    }

    private static FAQEntryDTO MapToDto(FAQEntry entry)
    {
        return new FAQEntryDTO(entry.Id, entry.Question, entry.Answer, entry.Category, entry.ViewCount, entry.HelpfulVotesCount, entry.NotHelpfulVotesCount);
    }
}
