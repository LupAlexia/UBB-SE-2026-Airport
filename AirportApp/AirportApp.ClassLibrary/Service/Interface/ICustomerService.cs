using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface ICustomerService
{
    Task<Customer?> GetByIdAsync(int customerId);
    Task<Customer?> GetByEmailAsync(string email);
    Task UpdateMembershipAsync(int customerId, int membershipId);
    Task AddAsync(Customer customer);
}
