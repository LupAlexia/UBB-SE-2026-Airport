using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Entity.Domain;

public class NullBotStrategy(IDecisionTreeService decisionTreeService) : IBotStrategy
{
    private const int RootNodeId = 1;

    public async Task<BotMessage> ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(
        BotEngineIdentity activeBotEngineInstance, IMessage incomingUserMessage)
    {
        FAQNode targetNode = await ResolveTargetNodeAsync(incomingUserMessage.Text);
        return new BotMessage.BotMessageBuilder(activeBotEngineInstance, incomingUserMessage.GetChat(), -1, targetNode).Build();
    }

    public Task ResetCurrentlyActiveConversationNodeToInitialStartingPointAsync()
    {
        return Task.CompletedTask;
    }

    private async Task<FAQNode> ResolveTargetNodeAsync(string incomingText)
    {
        if (string.IsNullOrWhiteSpace(incomingText))
        {
            return await decisionTreeService.GetNodeByIdAsync(RootNodeId);
        }

        string normalizedText = incomingText.Trim();

        return normalizedText switch
        {
            "Hello! I need help." => await decisionTreeService.GetNodeByIdAsync(RootNodeId),
            "Baggage Issues" => await decisionTreeService.GetNodeByIdAsync(2),
            "Seat Selection" => await decisionTreeService.GetNodeByIdAsync(4),
            "Lost baggage" => await decisionTreeService.GetNodeByIdAsync(5),
            "Damaged baggage" => await decisionTreeService.GetNodeByIdAsync(6),
            "Delayed baggage" => await decisionTreeService.GetNodeByIdAsync(7),
            _ => await decisionTreeService.GetNodeByIdAsync(RootNodeId)
        };
    }
}
