using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ChatService(IChatRepository chatRepository, IMessageRepository messageRepository) : IChatService
{
    public async Task<IEnumerable<Chat>> GetAllChatsAsync()
    {
        return await chatRepository.GetAsync();
    }

    public async Task<Chat?> GetChatByIdAsync(int chatId)
    {
        return await chatRepository.GetByIdAsync(chatId);
    }

    public async Task AddChatAsync(Chat chat)
    {
        await chatRepository.AddAsync(chat);
    }

    public async Task UpdateChatAsync(Chat chat)
    {
        await chatRepository.UpdateAsync(chat);
    }

    public async Task DeleteChatAsync(int chatId)
    {
        await chatRepository.DeleteAsync(chatId);
    }

    public async Task<IEnumerable<Message>> GetAllMessagesAsync()
    {
        return await messageRepository.GetAsync();
    }

    public async Task<Message?> GetMessageByIdAsync(int messageId)
    {
        return await messageRepository.GetByIdAsync(messageId);
    }

    public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(int chatId)
    {
        return await messageRepository.GetByChatIdAsync(chatId);
    }

    public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int startMessageId)
    {
        return await messageRepository.GetMessagesSinceAsync(chatId, startMessageId);
    }

    public async Task AddMessageAsync(Message message)
    {
        await messageRepository.AddAsync(message);
    }

    public async Task DeleteMessageAsync(int messageId)
    {
        await messageRepository.DeleteAsync(messageId);
    }
}
