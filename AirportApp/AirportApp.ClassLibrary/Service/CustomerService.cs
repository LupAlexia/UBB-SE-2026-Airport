using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<Customer?> GetByIdAsync(int customerId)
    {
        return await customerRepository.GetByIdAsync(customerId);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await customerRepository.GetByEmailAsync(email);
    }

    public async Task UpdateMembershipAsync(int customerId, int membershipId)
    {
        await customerRepository.UpdateMembershipAsync(customerId, membershipId);
    }

    public async Task AddAsync(Customer customer)
    {
        await customerRepository.AddAsync(customer);
    }
}
