using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    public async Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        return await clientRepository.GetAsync();
    }

    public async Task<Client?> GetClientByIdAsync(int clientId)
    {
        return await clientRepository.GetByIdAsync(clientId);
    }

    public async Task AddClientAsync(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Name))
            throw new ArgumentException("Client name cannot be empty.");
        await clientRepository.AddAsync(client);
    }

    public async Task UpdateClientAsync(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Name))
            throw new ArgumentException("Client name cannot be empty.");
        await clientRepository.UpdateAsync(client);
    }

    public async Task DeleteClientAsync(int clientId)
    {
        await clientRepository.DeleteAsync(clientId);
    }

    public async Task<Client> GetAnyClientAsync()
    {
        var clients = await clientRepository.GetAsync();
        return clients.FirstOrDefault()
            ?? throw new InvalidOperationException("No clients found.");
    }
}
