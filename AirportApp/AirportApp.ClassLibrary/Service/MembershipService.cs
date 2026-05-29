using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class MembershipService(ICustomerRepository userRepository, IMembershipRepository membershipRepository) : IMembershipService
{
    public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
    {
        var memberships = (await membershipRepository.GetAsync()).ToList();
        foreach (var membership in memberships)
        {
            membership.AddonDiscounts = (await membershipRepository.GetDiscountsByMembershipIdAsync(membership.Id)).ToList();
        }
        return memberships;
    }

    public async Task<Membership?> GetMembershipByIdAsync(int id)
    {
        return await membershipRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<MembershipAddonDiscount>> GetAddonDiscountsAsync(int membershipId)
    {
        return await membershipRepository.GetDiscountsByMembershipIdAsync(membershipId);
    }

    public async Task<Membership?> UpgradeUserMembershipAsync(int userId, int newMembershipId)
    {
        await userRepository.UpdateMembershipAsync(userId, newMembershipId);

        var membership = await membershipRepository.GetByIdAsync(newMembershipId);
        if (membership != null)
        {
            membership.AddonDiscounts = (await membershipRepository.GetDiscountsByMembershipIdAsync(newMembershipId)).ToList();
        }
        return membership;
    }

    public async Task<MembershipPurchaseResult> PurchaseMembershipAsync(int userId, int membershipId)
    {
        try
        {
            var updatedMembership = await UpgradeUserMembershipAsync(userId, membershipId);
            if (updatedMembership != null && UserSession.CurrentUser != null)
            {
                UserSession.CurrentUser.Membership = updatedMembership;
            }
            return new MembershipPurchaseResult
            {
                Succeeded = true,
                Message = "Your membership purchase was completed successfully."
            };
        }
        catch
        {
            return new MembershipPurchaseResult
            {
                Succeeded = false,
                Message = "Membership purchase could not be completed. Please try again."
            };
        }
    }
}
