using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class MessageService(
    IChatRepository chatRepository,
    IMessageRepository messageRepository,
    IDecisionTreeService decisionTreeService,
    BotEngineIdentity botEngine) : IMessageService
{
    public async Task<BotMessage> SendMessageAsync(int chatId, Sender sender, FAQOption selectedOption)
    {
        if (selectedOption == null)
            throw new ArgumentNullException(nameof(selectedOption));
        if (selectedOption.NextOption?.NodeId == 1)
            await botEngine.ResetBotConversationStateToInitialRootNodeAsync();

        Chat chat = await GetActiveChatAsync(chatId);

        var userMessage = new Message(chat, selectedOption.Label, sender);
        await messageRepository.AddAsync(userMessage);

        BotMessage botReply;
        if (selectedOption.NextOption != null)
        {
            FAQNode nextNode = await decisionTreeService.GetNodeByIdAsync(selectedOption.NextOption.NodeId);
            botReply = new BotMessage.BotMessageBuilder(botEngine, chat, -1, nextNode).Build();
        }
        else
        {
            botReply = await botEngine.GenerateAppropriateResponseBasedOnCurrentStrategyAsync(userMessage);
        }

        var botRow = new Message(chat, botReply.GetMessage(), botEngine);
        await messageRepository.AddAsync(botRow);

        return botReply;
    }

    public async Task<IMessage> GetMessageAsync(int chatId, int messageId)
    {
        IMessage message = await messageRepository.GetByIdAsync(messageId) ?? throw new KeyNotFoundException($"Message {messageId} not found.");
        if (message.GetChat().Id != chatId)
            throw new InvalidOperationException($"Message {messageId} does not belong to chat {chatId}.");
        return message;
    }

    public async Task<IEnumerable<Message>> GetAllMessagesAsync(int chatId)
    {
        var allMessages = await messageRepository.GetAsync();
        return allMessages
            .Where(chatMessage => chatMessage.Chat.Id == chatId)
            .OrderBy(chatMessage => chatMessage.Timestamp);
    }

    public async Task<IEnumerable<Message>> GetAllAsync()
    {
        return await messageRepository.GetAsync();
    }

    public async Task<Message> GetByIdAsync(int id)
    {
        return await messageRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Message {id} not found.");
    }

    public async Task<int> CreateMessageAsync(int chatId, int senderId, string text, DateTimeOffset timestamp)
    {
        Chat chat = await chatRepository.GetByIdAsync(chatId) ?? throw new KeyNotFoundException($"Chat {chatId} not found.");
        Sender sender = await messageRepository.GetSenderByIdAsync(senderId);
        
        var message = new Message(chat, text, sender)
        {
            Timestamp = timestamp == default ? DateTimeOffset.UtcNow : timestamp
        };

        return await messageRepository.AddAsync(message);
    }

    public async Task UpdateByIdAsync(int id, Message message)
    {
        await messageRepository.UpdateAsync(message);
    }

    public async Task DeleteByIdAsync(int id)
    {
        await messageRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Message>> GetByChatIdAsync(int chatId)
    {
        return await messageRepository.GetByChatIdAsync(chatId);
    }

    public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId)
    {
        return await messageRepository.GetMessagesSinceAsync(chatId, firstMessageId);
    }

    private async Task<Chat> GetActiveChatAsync(int chatId)
    {
        Chat chat = await chatRepository.GetByIdAsync(chatId) ?? throw new KeyNotFoundException($"Chat {chatId} not found.");
        if (chat.Status != ChatStatus.Active)
            throw new InvalidOperationException($"Chat {chatId} is not active.");
        return chat;
    }
}
