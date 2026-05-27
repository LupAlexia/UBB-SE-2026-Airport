using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class DecisionTreeRepository(AppDbContext databaseContext) : IDecisionTreeRepository
{
    private IQueryable<FAQNode> CompleteDecisionGraph => databaseContext.FaqNodes
        .Include(node => node.Options)
            .ThenInclude(option => option.NextOption);

    public async Task<IEnumerable<FAQNode>> GetAsync()
    {
        return await CompleteDecisionGraph
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<FAQNode?> GetByIdAsync(int nodeId)
    {
        var node = await CompleteDecisionGraph
            .FirstOrDefaultAsync(node => node.NodeId == nodeId);

        if (node is null)
        {
            throw new KeyNotFoundException($"Decision Node with id {nodeId} was not found.");
        }

        return node;
    }

    public async Task<int> AddAsync(FAQNode faqNode)
    {
        if (faqNode is null)
        {
            throw new ArgumentNullException(nameof(faqNode));
        }

        var nodeToPersist = new FAQNode
        {
            QuestionText = faqNode.QuestionText,
            IsFinalAnswer = faqNode.IsFinalAnswer
        };

        foreach (var option in faqNode.Options)
        {
            var optionEntity = new FAQOption(option.Label, option.NextOption);
            nodeToPersist.Options.Add(optionEntity);
        }

        databaseContext.FaqNodes.Add(nodeToPersist);
        await databaseContext.SaveChangesAsync();

        return nodeToPersist.NodeId;
    }

    public async Task UpdateAsync(FAQNode faqNode)
    {
        if (faqNode is null)
        {
            throw new ArgumentNullException(nameof(faqNode));
        }

        var existingNode = await databaseContext.FaqNodes
            .Include(node => node.Options)
            .FirstOrDefaultAsync(node => node.NodeId == faqNode.NodeId);

        if (existingNode is null)
        {
            return;
        }

        existingNode.QuestionText = faqNode.QuestionText;
        existingNode.IsFinalAnswer = faqNode.IsFinalAnswer;

        existingNode.Options.Clear();

        foreach (var option in faqNode.Options)
        {
            var optionEntity = new FAQOption(option.Label, option.NextOption);
            existingNode.Options.Add(optionEntity);
        }

        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int nodeId)
    {
        var nodeToRemove = await databaseContext.FaqNodes
            .Include(node => node.Options)
            .FirstOrDefaultAsync(node => node.NodeId == nodeId);

        if (nodeToRemove is not null)
        {
            databaseContext.FaqNodes.Remove(nodeToRemove);
            await databaseContext.SaveChangesAsync();
        }
    }
}