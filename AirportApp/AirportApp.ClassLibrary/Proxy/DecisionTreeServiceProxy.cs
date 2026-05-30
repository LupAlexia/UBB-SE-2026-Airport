using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class DecisionTreeServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IDecisionTreeService
{
    private const string BaseUrl = "api/decisiontree";

    public async Task<IEnumerable<FAQNode>> GetAllNodesAsync()
    {
        return await GetListAsync<FAQNode>(BaseUrl);
    }

    public async Task<FAQNode> GetNodeByIdAsync(int id)
    {
        return await GetRequiredAsync<FAQNode>($"{BaseUrl}/{id}");
    }

    public async Task<int> CreateNodeAsync(FAQNode node)
    {
        return await PostForResultAsync<FAQNode, int>(BaseUrl, node);
    }

    public async Task UpdateNodeAsync(int id, FAQNode node)
    {
        await PutAsync($"{BaseUrl}/{id}", node);
    }

    public async Task DeleteNodeAsync(int id)
    {
        await DeleteAsync($"{BaseUrl}/{id}");
    }
}
