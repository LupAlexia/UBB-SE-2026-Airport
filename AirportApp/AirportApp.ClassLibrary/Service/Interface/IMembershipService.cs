using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interface;

public interface IMembershipService
{
    Task<IEnumerable<Membership>> GetAllAsync();
    Task<Membership?> GetByIdAsync(int membershipId);
    Task<IEnumerable<MembershipAddonDiscount>> GetDiscountsByMembershipIdAsync(int membershipId);
}
