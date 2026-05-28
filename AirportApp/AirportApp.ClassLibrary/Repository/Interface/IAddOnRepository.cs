using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IAddOnRepository
{
    Task<IEnumerable<AddOn>> GetAsync();

    Task<IEnumerable<AddOn>> GetByIdsAsync(IEnumerable<int> addOnIds);
}