using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class ComplaintTicketSubcategoryServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IComplaintTicketSubcategoryService
{
    private const string BaseUrl = "api/complaintsubcategory";

    public async Task<IEnumerable<ComplaintTicketSubcategory>> GetAllSubcategoriesAsync()
    {
        return await GetListAsync<ComplaintTicketSubcategory>(BaseUrl);
    }

    public async Task<ComplaintTicketSubcategory> GetSubcategoryByIdAsync(int subcategoryId)
    {
        return await GetRequiredAsync<ComplaintTicketSubcategory>($"{BaseUrl}/{subcategoryId}");
    }

    public async Task<IEnumerable<ComplaintTicketSubcategory>> GetSubcategoriesByCategoryIdAsync(int categoryId)
    {
        return await GetListAsync<ComplaintTicketSubcategory>($"{BaseUrl}/category/{categoryId}");
    }
}
