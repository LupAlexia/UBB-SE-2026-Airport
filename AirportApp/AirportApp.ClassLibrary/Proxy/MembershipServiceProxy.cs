using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class MembershipServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IMembershipService
{
    private const string BaseUrl = "api/membership";

    public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
    {
        var dtos = await GetListAsync<MembershipDTO>(BaseUrl);
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<Membership?> GetMembershipByIdAsync(int id)
    {
        var dto = await GetOptionalAsync<MembershipDTO>($"{BaseUrl}/{id}");
        return dto is null ? null : MapToEntity(dto);
    }

    public async Task<IEnumerable<MembershipAddonDiscount>> GetAddonDiscountsAsync(int membershipId)
    {
        return await GetListAsync<MembershipAddonDiscount>($"{BaseUrl}/{membershipId}/discounts");
    }

    public async Task<Membership?> UpgradeUserMembershipAsync(int userId, int newMembershipId)
    {
        var dto = await PostForResultAsync<object, MembershipDTO?>($"{BaseUrl}/upgrade?userId={userId}&newMembershipId={newMembershipId}", null!);
        return dto is null ? null : MapToEntity(dto);
    }

    public async Task<MembershipPurchaseResult> PurchaseMembershipAsync(int userId, int membershipId)
    {
        return await PostForResultAsync<object, MembershipPurchaseResult>($"{BaseUrl}/purchase?userId={userId}&membershipId={membershipId}", null!);
    }

    private static Membership MapToEntity(MembershipDTO dto)
    {
        return new Membership(dto.id, dto.name, dto.flightDiscountPercentage);
    }
}
