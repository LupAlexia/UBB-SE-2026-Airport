using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository;

public class MembershipRepository(AppDbContext databaseContext) : IMembershipRepository
{
    private const string MembershipIdShadowProperty = "MembershipId";

    public async Task<IEnumerable<Membership>> GetAsync()
    {
        return await databaseContext.Memberships
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Membership?> GetByIdAsync(int membershipId)
    {
        return await databaseContext.Memberships
            .FirstOrDefaultAsync(membership => membership.Id == membershipId);
    }

    public async Task<IEnumerable<MembershipAddonDiscount>> GetDiscountsByMembershipIdAsync(int membershipId)
    {
        return await databaseContext.MembershipAddonDiscounts
            .Include(discount => discount.AddOn)
            .Where(discount => EF.Property<int>(discount, MembershipIdShadowProperty) == membershipId)
            .AsNoTracking()
            .ToListAsync();
    }
}