using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class MessageRepository(AppDbContext databaseContext) : IMessageRepository
{
    private const string ChatIdProperty = "ChatId";
    private const string SenderIdProperty = "SenderId";
    private const int BotSystemUserId = BotEngineIdentity.CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER;

    public async Task<IEnumerable<Message>> GetAsync()
    {
        var messages = await databaseContext.Messages
            .Include(message => message.Chat)
            .AsNoTracking()
            .ToListAsync();

        await ResolveSendersForCollectionAsync(messages);
        return messages;
    }

    public async Task<Message?> GetByIdAsync(int messageId)
    {
        var message = await databaseContext.Messages
            .Include(message => message.Chat)
            .FirstOrDefaultAsync(message => message.Id == messageId);

        if (message is not null)
        {
            message.Sender = await ResolveSenderAsync(message);
        }

        return message;
    }

    public async Task<IEnumerable<Message>> GetByChatIdAsync(int chatId)
    {
        var messages = await databaseContext.Messages
            .Include(message => message.Chat)
            .Where(message => EF.Property<int>(message, ChatIdProperty) == chatId)
            .OrderBy(message => message.Timestamp)
            .ToListAsync();

        await ResolveSendersForCollectionAsync(messages);
        return messages;
    }

    public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int startMessageId)
    {
        var messages = await databaseContext.Messages
            .Include(message => message.Chat)
            .Where(message => EF.Property<int>(message, ChatIdProperty) == chatId && message.Id >= startMessageId)
            .OrderBy(message => message.Timestamp)
            .ToListAsync();

        await ResolveSendersForCollectionAsync(messages);
        return messages;
    }

    public async Task<int> AddAsync(Message message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var chatId = message.Chat?.Id ?? throw new InvalidOperationException("A message requires a valid Chat reference.");
        var senderId = message.Sender?.RetrieveUniqueDatabaseIdentifierForBot() ?? throw new InvalidOperationException("A message requires a valid Sender.");

        databaseContext.Entry(message).Property(ChatIdProperty).CurrentValue = chatId;
        databaseContext.Entry(message).Property(SenderIdProperty).CurrentValue = senderId;

        message.Chat = null!;
        message.Sender = null!;

        databaseContext.Messages.Add(message);
        await databaseContext.SaveChangesAsync();

        return message.Id;
    }

    public async Task DeleteAsync(int messageId)
    {
        var messageToRemove = await databaseContext.Messages.FindAsync(messageId);
        if (messageToRemove is null)
        {
            return;
        }

        databaseContext.Messages.Remove(messageToRemove);
        await databaseContext.SaveChangesAsync();
    }

    private async Task ResolveSendersForCollectionAsync(IEnumerable<Message> messages)
    {
        foreach (var message in messages)
        {
            message.Sender = await ResolveSenderAsync(message);
        }
    }

    private async Task<Sender> ResolveSenderAsync(Message message)
    {
        var senderId = databaseContext.Entry(message).Property<int>(SenderIdProperty).CurrentValue;

        if (senderId == BotSystemUserId)
        {
            return new BotEngineIdentity(null!);
        }

        return await databaseContext.Senders.FindAsync(senderId)
               ?? throw new KeyNotFoundException($"Sender with ID {senderId} not found in the database.");
    }
}