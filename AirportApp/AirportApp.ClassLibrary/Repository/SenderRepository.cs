using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class SenderRepository(AppDbContext databaseContext) : ISenderRepository
{
    private const int BotSentinelId = BotEngineIdentity.CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER;

    public async Task<IEnumerable<Sender>> GetAsync()
    {
        return await databaseContext.Senders
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Sender?> GetByIdAsync(int senderId)
    {
        if (senderId == BotSentinelId)
        {
            return new BotEngineIdentity(null!);
        }

        var sender = await databaseContext.Senders.FindAsync(senderId);

        if (sender is null)
        {
            throw new KeyNotFoundException($"Sender with id {senderId} was not found.");
        }

        return sender;
    }

    public async Task<int> AddAsync(Sender sender)
    {
        if (sender is null)
        {
            throw new ArgumentNullException(nameof(sender));
        }

        sender.Id = 0;
        databaseContext.Senders.Add(sender);
        await databaseContext.SaveChangesAsync();

        return sender.Id;
    }

    public async Task UpdateAsync(Sender sender)
    {
        if (sender is null)
        {
            throw new ArgumentNullException(nameof(sender));
        }

        databaseContext.Senders.Update(sender);
        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int senderId)
    {
        var senderToRemove = await databaseContext.Senders.FindAsync(senderId);

        if (senderToRemove is not null)
        {
            databaseContext.Senders.Remove(senderToRemove);
            await databaseContext.SaveChangesAsync();
        }
    }
}