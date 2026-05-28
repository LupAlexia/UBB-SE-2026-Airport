using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IChatService
{
    Task<IEnumerable<Chat>> GetAllChatsAsync();
    Task<Chat?> GetChatByIdAsync(int chatId);
    Task AddChatAsync(Chat chat);
    Task UpdateChatAsync(Chat chat);
    Task DeleteChatAsync(int chatId);
    Task<IEnumerable<Message>> GetAllMessagesAsync();
    Task<Message?> GetMessageByIdAsync(int messageId);
    Task<IEnumerable<Message>> GetMessagesByChatIdAsync(int chatId);
    Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int startMessageId);
    Task AddMessageAsync(Message message);
    Task DeleteMessageAsync(int messageId);
}
