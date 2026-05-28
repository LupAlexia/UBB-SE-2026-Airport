using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class CompanyRepository(AppDbContext databaseContext) : ICompanyRepository
{
    public async Task<IEnumerable<Company>> GetAsync()
    {
        return await databaseContext.Companies
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Company?> GetByIdAsync(int companyId)
    {
        return await databaseContext.Companies.FindAsync(companyId);
    }

    public async Task<int> AddAsync(Company company)
    {
        if (company is null)
        {
            throw new ArgumentNullException(nameof(company));
        }

        company.Id = 0;
        databaseContext.Companies.Add(company);
        await databaseContext.SaveChangesAsync();

        return company.Id;
    }

    public async Task UpdateAsync(Company company)
    {
        if (company is null)
        {
            throw new ArgumentNullException(nameof(company));
        }

        var existingCompany = await databaseContext.Companies.FindAsync(company.Id);

        if (existingCompany is null)
        {
            return;
        }

        existingCompany.Name = company.Name;

        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int companyId)
    {
        var companyToRemove = await databaseContext.Companies.FindAsync(companyId);

        if (companyToRemove is null)
        {
            return;
        }

        databaseContext.Companies.Remove(companyToRemove);
        await databaseContext.SaveChangesAsync();
    }
}