using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class MembershipServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IMembershipService
{
    private const string BaseUrl = "api/membership";

    public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
    {
        return await GetListAsync<Membership>(BaseUrl);
    }

    public async Task<Membership?> GetMembershipByIdAsync(int id)
    {
        return await GetOptionalAsync<Membership>($"{BaseUrl}/{id}");
    }

    public async Task<IEnumerable<MembershipAddonDiscount>> GetAddonDiscountsAsync(int membershipId)
    {
        return await GetListAsync<MembershipAddonDiscount>($"{BaseUrl}/{membershipId}/discounts");
    }

    public async Task<Membership?> UpgradeUserMembershipAsync(int userId, int newMembershipId)
    {
        return await PostForResultAsync<object, Membership?>($"{BaseUrl}/upgrade?userId={userId}&newMembershipId={newMembershipId}", null!);
    }

    public async Task<MembershipPurchaseResult> PurchaseMembershipAsync(int userId, int membershipId)
    {
        return await PostForResultAsync<object, MembershipPurchaseResult>($"{BaseUrl}/purchase?userId={userId}&membershipId={membershipId}", null!);
    }
}
