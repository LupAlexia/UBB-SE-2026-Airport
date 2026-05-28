using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IRunwayService
{
    Task<IEnumerable<Runway>> GetAllRunwaysAsync();
    Task<Runway?> GetRunwayByIdAsync(int runwayId);
    Task AddRunwayAsync(Runway runway);
    Task UpdateRunwayAsync(Runway runway);
    Task DeleteRunwayAsync(int runwayId);
    Task<bool> HasFlightsAsync(int runwayId);
    Task SaveRunwayAsync(string name, string handleTimeText, int existingId = 0);
}
