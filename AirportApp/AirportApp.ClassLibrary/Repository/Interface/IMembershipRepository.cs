using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IMembershipRepository
{
    Task<IEnumerable<Membership>> GetAsync();

    Task<Membership?> GetByIdAsync(int membershipId);

    Task<IEnumerable<MembershipAddonDiscount>> GetDiscountsByMembershipIdAsync(int membershipId);
}