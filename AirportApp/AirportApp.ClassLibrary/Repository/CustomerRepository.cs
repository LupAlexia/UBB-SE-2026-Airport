using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class CustomerRepository(
    AppDbContext databaseContext,
    IMembershipRepository membershipRepository) : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(int customerId)
    {
        return await databaseContext.Customers
            .Include(customer => customer.Membership)
            .FirstOrDefaultAsync(customer => customer.Id == customerId);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return await databaseContext.Customers
            .Include(customer => customer.Membership)
            .FirstOrDefaultAsync(customer => customer.Email == email);
    }

    public async Task AddAsync(Customer customer)
    {
        if (customer is null)
        {
            throw new ArgumentNullException(nameof(customer));
        }

        databaseContext.Customers.Add(customer);
        await databaseContext.SaveChangesAsync();
    }

    public async Task UpdateMembershipAsync(int customerId, int membershipId)
    {
        var customerToUpdate = await databaseContext.Customers
            .Include(customer => customer.Membership)
            .FirstOrDefaultAsync(customer => customer.Id == customerId);

        if (customerToUpdate is null)
        {
            throw new KeyNotFoundException($"Customer with id {customerId} was not found.");
        }

        var membership = await membershipRepository.GetByIdAsync(membershipId);

        if (membership is null)
        {
            throw new KeyNotFoundException($"Membership level {membershipId} does not exist.");
        }

        customerToUpdate.Membership = membership;

        await databaseContext.SaveChangesAsync();
    }
}