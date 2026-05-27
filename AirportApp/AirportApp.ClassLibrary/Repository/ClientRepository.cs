using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class ClientRepository(AppDbContext databaseContext) : IClientRepository
{
    public async Task<IEnumerable<Client>> GetAsync()
    {
        return await databaseContext.Clients
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Client?> GetByIdAsync(int clientId)
    {
        return await databaseContext.Clients.FindAsync(clientId);
    }

    public async Task<int> AddAsync(Client client)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        client.Id = 0;
        databaseContext.Clients.Add(client);
        await databaseContext.SaveChangesAsync();

        return client.Id;
    }

    public async Task UpdateAsync(Client client)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        var existingClient = await databaseContext.Clients.FindAsync(client.Id);

        if (existingClient is null)
        {
            return;
        }

        existingClient.Name = client.Name;

        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int clientId)
    {
        var clientToRemove = await databaseContext.Clients.FindAsync(clientId);

        if (clientToRemove is null)
        {
            return;
        }

        databaseContext.Clients.Remove(clientToRemove);
        await databaseContext.SaveChangesAsync();
    }
}