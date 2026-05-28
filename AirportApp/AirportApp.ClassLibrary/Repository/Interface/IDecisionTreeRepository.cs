using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IDecisionTreeRepository
{
    Task<IEnumerable<FAQNode>> GetAsync();
    Task<FAQNode?> GetByIdAsync(int nodeId);
    Task<int> AddAsync(FAQNode faqNode);
    Task UpdateAsync(FAQNode faqNode);
    Task DeleteAsync(int nodeId);
}