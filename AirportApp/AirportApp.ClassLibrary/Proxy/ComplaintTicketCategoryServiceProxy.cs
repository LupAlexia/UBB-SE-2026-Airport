using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class ComplaintTicketCategoryServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IComplaintTicketCategoryService
{
    private const string BaseUrl = "api/ticketcategory";

    public async Task<ComplaintTicketCategory> GetCategoryByIdAsync(int categoryId)
    {
        return await GetRequiredAsync<ComplaintTicketCategory>($"{BaseUrl}/{categoryId}");
    }

    public async Task<IEnumerable<ComplaintTicketCategory>> GetAllCategoriesAsync()
    {
        return await GetListAsync<ComplaintTicketCategory>(BaseUrl);
    }
}
