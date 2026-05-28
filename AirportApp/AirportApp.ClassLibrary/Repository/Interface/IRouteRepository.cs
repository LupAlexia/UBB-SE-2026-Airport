using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IRouteRepository
{
    Task<IEnumerable<Route>> GetAsync();
    Task<Route?> GetByIdAsync(int routeId);
    Task<int> AddAsync(Route route);
    Task UpdateAsync(Route route);
    Task DeleteAsync(int routeId);
}