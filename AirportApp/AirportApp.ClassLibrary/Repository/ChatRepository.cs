using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class ChatRepository(AppDbContext databaseContext) : IChatRepository
{
    private const string UserIdProperty = "UserId";

    public async Task<IEnumerable<Chat>> GetAsync()
    {
        return await databaseContext.Chats
            .Include(chat => chat.User)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Chat?> GetByIdAsync(int chatId)
    {
        var chat = await databaseContext.Chats
            .Include(chat => chat.User)
            .FirstOrDefaultAsync(chat => chat.Id == chatId);

        if (chat is null)
        {
            throw new KeyNotFoundException($"Chat with id {chatId} was not found.");
        }

        return chat;
    }

    public async Task<int> AddAsync(Chat chat)
    {
        if (chat is null)
        {
            throw new ArgumentNullException(nameof(chat));
        }

        if (chat.User is null || chat.User.Id <= 0)
        {
            throw new ArgumentException("A new chat must be associated with a valid user.", nameof(chat));
        }

        var chatToPersist = new Chat
        {
            Status = chat.Status
        };

        databaseContext.Chats.Add(chatToPersist);

        databaseContext.Entry(chatToPersist)
            .Property(UserIdProperty)
            .CurrentValue = chat.User.Id;

        await databaseContext.SaveChangesAsync();
        return chatToPersist.Id;
    }

    public async Task UpdateAsync(Chat chat)
    {
        if (chat is null)
        {
            throw new ArgumentNullException(nameof(chat));
        }

        var existingChat = await databaseContext.Chats
            .FirstOrDefaultAsync(chatEntity => chatEntity.Id == chat.Id);

        if (existingChat is not null)
        {
            existingChat.Status = chat.Status;
            await databaseContext.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int chatId)
    {
        var chatToRemove = await databaseContext.Chats.FindAsync(chatId);

        if (chatToRemove is not null)
        {
            databaseContext.Chats.Remove(chatToRemove);
            await databaseContext.SaveChangesAsync();
        }
    }
}