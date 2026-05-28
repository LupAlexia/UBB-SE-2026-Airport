using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class GateService(IGateRepository gateRepository, IFlightRepository flightRepository) : IGateService
{
    public async Task<IEnumerable<Gate>> GetAllGatesAsync()
    {
        return await gateRepository.GetAsync();
    }

    public async Task<Gate?> GetGateByIdAsync(int gateId)
    {
        return await gateRepository.GetByIdAsync(gateId);
    }

    public async Task AddGateAsync(Gate gate)
    {
        if (string.IsNullOrWhiteSpace(gate.GateName))
            throw new ArgumentException("Gate name cannot be empty.");
        await gateRepository.AddAsync(gate);
    }

    public async Task UpdateGateAsync(Gate gate)
    {
        await gateRepository.UpdateAsync(gate);
    }

    public async Task DeleteGateAsync(int gateId)
    {
        await gateRepository.DeleteAsync(gateId);
    }

    public async Task<bool> HasFlightsAsync(int gateId)
    {
        var flights = await flightRepository.GetByGateIdAsync(gateId);
        return flights.Any();
    }

    public async Task<string> GetDeleteWarningMessageAsync(int gateId, string gateName)
    {
        bool hasFlights = await HasFlightsAsync(gateId);
        if (hasFlights)
            return string.Format("CRITICAL: Gate '{0}' has flights assigned. Deleting it will remove all associated flights. Are you sure?", gateName);
        return string.Format("Are you sure you want to delete gate '{0}'?", gateName);
    }

    public async Task SaveGateAsync(Gate gate)
    {
        if (string.IsNullOrWhiteSpace(gate.GateName))
            throw new ArgumentException("Gate name cannot be empty.");

        if (gate.Id == 0)
            await gateRepository.AddAsync(gate);
        else
            await gateRepository.UpdateAsync(gate);
    }
}
