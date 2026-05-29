using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class DecisionTreeService(IDecisionTreeRepository decisionTreeRepository) : IDecisionTreeService
{
    public async Task<IEnumerable<FAQNode>> GetAllNodesAsync()
    {
        return await decisionTreeRepository.GetAsync();
    }

    public async Task<FAQNode> GetNodeByIdAsync(int id)
    {
        return await decisionTreeRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Node {id} not found.");
    }

    public async Task<int> CreateNodeAsync(FAQNode node)
    {
        return await decisionTreeRepository.AddAsync(node);
    }

    public async Task UpdateNodeAsync(int id, FAQNode node)
    {
        await decisionTreeRepository.UpdateAsync(node);
    }

    public async Task DeleteNodeAsync(int id)
    {
        await decisionTreeRepository.DeleteAsync(id);
    }
}
