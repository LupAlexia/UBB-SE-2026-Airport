using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetAsync();
    Task<Message?> GetByIdAsync(int messageId);
    Task<IEnumerable<Message>> GetByChatIdAsync(int chatId);
    Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int startMessageId);
    Task<int> AddAsync(Message message);
    Task UpdateAsync(Message message);
    Task DeleteAsync(int messageId);
    Task<Sender> GetSenderByIdAsync(int senderId);
}