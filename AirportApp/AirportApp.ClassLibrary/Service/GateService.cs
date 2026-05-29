using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class GateService(IGateRepository gateRepository, IFlightRepository flightRepository) : IGateService
{
    private const string EmptyGateNameErrorMessage = "The gate name cannot be empty.";
    private const string CriticalDeleteWarningTemplate = "CRITICAL: Gate '{0}' has flights assigned. Deleting it will remove ALL associated flights. Proceed?";
    private const string StandardDeleteWarningTemplate = "Are you sure you want to delete gate '{0}'?";

    public async Task<IEnumerable<Gate>> GetAllGatesAsync()
    {
        return await gateRepository.GetAsync();
    }

    public async Task<Gate?> GetGateByIdAsync(int gateId)
    {
        if (gateId <= 0)
        {
            return null;
        }

        return await gateRepository.GetByIdAsync(gateId);
    }

    public async Task AddGateAsync(Gate gate)
    {
        if (string.IsNullOrWhiteSpace(gate.GateName))
        {
            throw new ArgumentException(EmptyGateNameErrorMessage, nameof(gate));
        }

        await gateRepository.AddAsync(gate);
    }

    public async Task UpdateGateAsync(Gate gate)
    {
        Gate? existingGate = await gateRepository.GetByIdAsync(gate.Id);

        if (existingGate == null)
        {
            return;
        }

        if (gate.GateName != null)
        {
            if (string.IsNullOrWhiteSpace(gate.GateName))
            {
                throw new ArgumentException(EmptyGateNameErrorMessage, nameof(gate));
            }

            existingGate.GateName = gate.GateName;
        }

        await gateRepository.UpdateAsync(existingGate);
    }

    public async Task DeleteGateAsync(int gateId)
    {
        if (gateId > 0)
        {
            await gateRepository.DeleteAsync(gateId);
        }
    }

    public async Task SaveGateAsync(Gate gate)
    {
        if (gate.Id == 0)
        {
            await AddGateAsync(gate);
        }
        else
        {
            await UpdateGateAsync(gate);
        }
    }

    public async Task<bool> HasFlightsAsync(int gateId)
    {
        IEnumerable<Flight> associatedFlights = await flightRepository.GetByGateIdAsync(gateId);
        return associatedFlights.Any();
    }

    public async Task<string> GetDeleteWarningMessageAsync(int gateId)
    {
        Gate? gate = await GetGateByIdAsync(gateId);

        if (gate == null)
        {
            return string.Empty;
        }

        bool gateHasAssignedFlights = await HasFlightsAsync(gateId);

        if (gateHasAssignedFlights)
        {
            return string.Format(CriticalDeleteWarningTemplate, gate.GateName);
        }

        return string.Format(StandardDeleteWarningTemplate, gate.GateName);
    }
}
