using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirportApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            this.membershipService = membershipService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.MembershipDTO>>> GetAllAsync()
        {
            IEnumerable<Membership> memberships = await membershipService.GetAllMembershipsAsync();
            var membershipTransferObjectList = new List<AirportApp.ClassLibrary.Entity.Dto.MembershipDTO>();
            foreach (var membership in memberships)
            {
                membershipTransferObjectList.Add(new AirportApp.ClassLibrary.Entity.Dto.MembershipDTO(membership.Id, membership.Name, membership.FlightDiscountPercentage));
            }
            return Ok(membershipTransferObjectList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AirportApp.ClassLibrary.Entity.Dto.MembershipDTO>> GetByIdAsync(int id)
        {
            Membership? membership = await membershipService.GetMembershipByIdAsync(id);
            if (membership == null)
            {
                return NotFound();
            }

            var membershipTransferObject = new AirportApp.ClassLibrary.Entity.Dto.MembershipDTO(membership.Id, membership.Name, membership.FlightDiscountPercentage);
            return Ok(membershipTransferObject);
        }

        [HttpGet("{id}/addon-discounts")]
        public async Task<ActionResult<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.MembershipAddonDiscountDTO>>> GetAddonDiscountsAsync(int id)
        {
            IEnumerable<MembershipAddonDiscount> discounts = await membershipService.GetAddonDiscountsAsync(id);
            var membershipAddonDiscountTransferObjectList = new List<AirportApp.ClassLibrary.Entity.Dto.MembershipAddonDiscountDTO>();
            foreach (var discount in discounts)
            {
                membershipAddonDiscountTransferObjectList.Add(new AirportApp.ClassLibrary.Entity.Dto.MembershipAddonDiscountDTO(
                    id,
                    discount.AddOn?.Id ?? 0,
                    discount.DiscountPercentage,
                    discount.AddOn?.Name ?? string.Empty));
            }
            return Ok(membershipAddonDiscountTransferObjectList);
        }

        [HttpPost("purchase")]
        public async Task<ActionResult<MembershipPurchaseResult>> PurchaseAsync([FromQuery] int userId, [FromQuery] int membershipId)
        {
            MembershipPurchaseResult result = await membershipService.PurchaseMembershipAsync(userId, membershipId);
            return Ok(result);
        }
    }
}