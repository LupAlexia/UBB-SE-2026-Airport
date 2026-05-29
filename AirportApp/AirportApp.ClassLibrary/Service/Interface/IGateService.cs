using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IGateService
{
    Task<IEnumerable<Gate>> GetAllGatesAsync();
    Task<Gate?> GetGateByIdAsync(int gateId);
    Task AddGateAsync(Gate gate);
    Task UpdateGateAsync(Gate gate);
    Task DeleteGateAsync(int gateId);
    Task<bool> HasFlightsAsync(int gateId);
    Task<string> GetDeleteWarningMessageAsync(int gateId);
    Task SaveGateAsync(Gate gate);
}
