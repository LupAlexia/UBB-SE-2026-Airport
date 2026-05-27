using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int customerId);

    Task<Customer?> GetByEmailAsync(string email);

    Task AddAsync(Customer customer);

    Task UpdateMembershipAsync(int customerId, int membershipId);
}