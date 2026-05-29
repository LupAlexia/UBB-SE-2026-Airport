using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IChatService
{
    Task<Chat> OpenChatAsync(User userToOpenChatFor);
    Task CloseChatAsync(int chatId);
    Task<IEnumerable<Chat>> GetAllChatsAsync();
    Task<Chat> GetChatByIdAsync(int id);
    Task UpdateChatAsync(int id, Chat chat);
}
