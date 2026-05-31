namespace AirportApp.ClassLibrary.Entity.Domain;

public class NullBotStrategy : IBotStrategy
{
    public Task<BotMessage> ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(
        BotEngineIdentity activeBotEngineInstance, IMessage incomingUserMessage)
    {
        throw new NotImplementedException("Bot strategy is not implemented.");
    }

    public Task ResetCurrentlyActiveConversationNodeToInitialStartingPointAsync()
    {
        return Task.CompletedTask;
    }
}
