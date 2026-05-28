using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class RunwayService(IRunwayRepository runwayRepository, IFlightRepository flightRepository) : IRunwayService
{
    public async Task<IEnumerable<Runway>> GetAllRunwaysAsync()
    {
        return await runwayRepository.GetAsync();
    }

    public async Task<Runway?> GetRunwayByIdAsync(int runwayId)
    {
        return await runwayRepository.GetByIdAsync(runwayId);
    }

    public async Task AddRunwayAsync(Runway runway)
    {
        if (string.IsNullOrWhiteSpace(runway.Name))
            throw new ArgumentException("Runway name cannot be empty.");
        if (runway.HandleTime <= 0)
            throw new ArgumentException("Handle time must be greater than 0.");
        await runwayRepository.AddAsync(runway);
    }

    public async Task UpdateRunwayAsync(Runway runway)
    {
        await runwayRepository.UpdateAsync(runway);
    }

    public async Task DeleteRunwayAsync(int runwayId)
    {
        var runway = await runwayRepository.GetByIdAsync(runwayId);
        if (runway is null)
            throw new InvalidOperationException($"Runway with ID {runwayId} not found.");
        await runwayRepository.DeleteAsync(runwayId);
    }

    public async Task<bool> HasFlightsAsync(int runwayId)
    {
        var flights = await flightRepository.GetByRunwayIdAsync(runwayId);
        return flights.Any();
    }

    public async Task SaveRunwayAsync(string name, string handleTimeText, int existingId = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Runway name cannot be empty.");
        if (!int.TryParse(handleTimeText, out int handleTime) || handleTime <= 0)
            throw new ArgumentException("Handle time must be a positive integer.");

        var runway = new Runway { Id = existingId, Name = name, HandleTime = handleTime };

        if (existingId == 0)
            await runwayRepository.AddAsync(runway);
        else
            await runwayRepository.UpdateAsync(runway);
    }
}
