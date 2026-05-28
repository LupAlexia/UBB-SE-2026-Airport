using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class MembershipService(IMembershipRepository membershipRepository) : IMembershipService
{
    public async Task<IEnumerable<Membership>> GetAllAsync()
    {
        return await membershipRepository.GetAsync();
    }

    public async Task<Membership?> GetByIdAsync(int membershipId)
    {
        return await membershipRepository.GetByIdAsync(membershipId);
    }

    public async Task<IEnumerable<MembershipAddonDiscount>> GetDiscountsByMembershipIdAsync(int membershipId)
    {
        return await membershipRepository.GetDiscountsByMembershipIdAsync(membershipId);
    }
}
