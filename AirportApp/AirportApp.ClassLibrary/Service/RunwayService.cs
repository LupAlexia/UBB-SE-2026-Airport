using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class RunwayService(IRunwayRepository runwayRepository, IFlightRepository flightRepository) : IRunwayService
{
    private const string EmptyRunwayNameErrorMessage = "The runway name cannot be empty.";
    private const string RunwayNotFoundErrorMessage = "Runway with Id {0} does not exist in the system.";
    private const string CriticalDeleteWarningTemplate = "CRITICAL: Runway '{0}' has flights assigned. Deleting it will remove ALL associated flights. Proceed?";
    private const string StandardDeleteWarningTemplate = "Are you sure you want to delete runway '{0}'?";

    public async Task<IEnumerable<Runway>> GetAllRunwaysAsync()
    {
        return await runwayRepository.GetAsync();
    }

    public async Task<Runway?> GetRunwayByIdAsync(int runwayId)
    {
        if (runwayId <= 0)
        {
            return null;
        }

        return await runwayRepository.GetByIdAsync(runwayId);
    }

    public async Task AddRunwayAsync(Runway runway)
    {
        if (string.IsNullOrWhiteSpace(runway.Name))
        {
            throw new ArgumentException(EmptyRunwayNameErrorMessage, nameof(runway));
        }

        if (runway.HandleTime <= 0)
        {
            throw new ArgumentException("The handle time must be a positive number greater than zero.", nameof(runway));
        }

        await runwayRepository.AddAsync(runway);
    }

    public async Task UpdateRunwayAsync(Runway runway)
    {
        Runway? existingRunway = await runwayRepository.GetByIdAsync(runway.Id);

        if (existingRunway == null)
        {
            throw new InvalidOperationException(string.Format(RunwayNotFoundErrorMessage, runway.Id));
        }

        if (runway.Name != null)
        {
            if (string.IsNullOrWhiteSpace(runway.Name))
            {
                throw new ArgumentException(EmptyRunwayNameErrorMessage, nameof(runway));
            }

            existingRunway.Name = runway.Name;
        }

        if (runway.HandleTime != 0)
        {
            if (runway.HandleTime <= 0)
            {
                throw new ArgumentException("The handle time must be a positive number greater than zero.", nameof(runway));
            }

            existingRunway.HandleTime = runway.HandleTime;
        }

        await runwayRepository.UpdateAsync(existingRunway);
    }

    public async Task DeleteRunwayAsync(int runwayId)
    {
        if (await runwayRepository.GetByIdAsync(runwayId) == null)
        {
            throw new InvalidOperationException(string.Format(RunwayNotFoundErrorMessage, runwayId));
        }

        await runwayRepository.DeleteAsync(runwayId);
    }

    public async Task SaveRunwayAsync(int runwayId, string runwayName, string handleTimeText)
    {
        if (!int.TryParse(handleTimeText, out int handleTime) || handleTime <= 0)
        {
            throw new ArgumentException("Handle time must be a valid positive numeric value.");
        }

        if (runwayId == 0)
        {
            Runway newRunway = new Runway { Name = runwayName, HandleTime = handleTime };
            await AddRunwayAsync(newRunway);
        }
        else
        {
            Runway updatedRunway = new Runway { Id = runwayId, Name = runwayName, HandleTime = handleTime };
            await UpdateRunwayAsync(updatedRunway);
        }
    }

    public async Task<bool> HasFlightsAsync(int runwayId)
    {
        IEnumerable<Flight> associatedFlights = await flightRepository.GetByRunwayIdAsync(runwayId);
        return associatedFlights.Any();
    }

    public async Task<string> GetDeleteWarningMessageAsync(int runwayId)
    {
        Runway? runway = await GetRunwayByIdAsync(runwayId);

        if (runway == null)
        {
            return string.Empty;
        }

        bool runwayHasAssignedFlights = await HasFlightsAsync(runwayId);

        if (runwayHasAssignedFlights)
        {
            return string.Format(CriticalDeleteWarningTemplate, runway.Name);
        }

        return string.Format(StandardDeleteWarningTemplate, runway.Name);
    }
}
