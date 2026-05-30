using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricingController : ControllerBase
    {
        private readonly IPricingService pricingService;

        public PricingController(IPricingService pricingService)
        {
            this.pricingService = pricingService;
        }

        [HttpPost("calculate-base-price")]
        public async Task<ActionResult<float>> CalculateBasePriceAsync([FromBody] CalculateBasePriceRequestDTO req)
        {
            var flight = new Flight
            {
                Route = new AirportApp.ClassLibrary.Entity.Domain.Route
                {
                    DepartureTime = TimeOnly.FromDateTime(req.DepartureTime),
                    ArrivalTime = TimeOnly.FromDateTime(req.ArrivalTime)
                }
            };
            float result = await pricingService.CalculateBasePriceAsync(flight);
            return Ok(result);
        }

        [HttpPost("calculate-total-price")]
        public async Task<ActionResult<float>> CalculateTotalPriceAsync([FromBody] CalculateTotalPriceRequestDTO req)
        {
            var ticket = new FlightTicket
            {
                Price = req.BasePrice,
                User = new Customer
                {
                    Membership = new Membership
                    {
                        FlightDiscountPercentage = req.FlightDiscountPercentage,
                        AddonDiscounts = req.AddonDiscounts?.Select(discount => new MembershipAddonDiscount
                        {
                            AddOn = new AddOn { Id = discount.AddOnId },
                            DiscountPercentage = discount.DiscountPercentage
                        }).ToList() ?? new List<MembershipAddonDiscount>()
                    }
                },
                SelectedAddOns = req.SelectedAddOns?.Select(addon => new AddOn { Id = addon.Id, BasePrice = addon.BasePrice }).ToList() ?? new List<AddOn>()
            };
            float result = await pricingService.CalculateTotalPriceAsync(ticket);
            return Ok(result);
        }

        [HttpPost("calculate-breakdown")]
        public async Task<ActionResult<PriceBreakdown>> CalculatePriceBreakdownAsync([FromBody] CalculatePriceBreakdownRequestDTO req)
        {
            var flight = new Flight
            {
                Route = new AirportApp.ClassLibrary.Entity.Domain.Route
                {
                    DepartureTime = TimeOnly.FromDateTime(req.FlightData.DepartureTime),
                    ArrivalTime = TimeOnly.FromDateTime(req.FlightData.ArrivalTime)
                }
            };

            var user = new Customer
            {
                Membership = new Membership
                {
                    FlightDiscountPercentage = req.FlightDiscountPercentage,
                    AddonDiscounts = req.AddonDiscounts?.Select(discount => new MembershipAddonDiscount
                    {
                        AddOn = new AddOn { Id = discount.AddOnId },
                        DiscountPercentage = discount.DiscountPercentage
                    }).ToList() ?? new List<MembershipAddonDiscount>()
                }
            };

            var tickets = new List<FlightTicket>();
            if (req.TicketsAddOns != null)
            {
                foreach (var addonList in req.TicketsAddOns)
                {
                    tickets.Add(new FlightTicket
                    {
                        SelectedAddOns = addonList?.Select(addon => new AddOn { Id = addon.Id, BasePrice = addon.BasePrice }).ToList() ?? new List<AddOn>()
                    });
                }
            }

            PriceBreakdown result = await pricingService.CalculatePriceBreakdownAsync(flight, user, tickets);
            return Ok(result);
        }
    }
}