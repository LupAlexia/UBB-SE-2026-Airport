using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Web.Models.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class BookingController : Controller
    {
        private const int DefaultFlightCapacity = 180;

        private readonly IFlightSearchService flightSearchService;
        private readonly IBookingService bookingService;
        private readonly IPricingService pricingService;
        private readonly IAuthService authService;
        private readonly IMembershipService membershipService;

        public BookingController(
            IFlightSearchService flightSearchService,
            IBookingService bookingService,
            IPricingService pricingService,
            IAuthService authService,
            IMembershipService membershipService)
        {
            this.flightSearchService = flightSearchService;
            this.bookingService = bookingService;
            this.pricingService = pricingService;
            this.authService = authService;
            this.membershipService = membershipService;
        }

        private int? GetCurrentUserId()
        {
            string? claimUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claimUserId, out int parsedId))
            {
                return parsedId;
            }

            return UserSession.CurrentUser?.Id;
        }

        private async Task<Customer> GetCustomerWithMembershipAsync(int userId)
        {
            try
            {
                Customer customer = await authService.GetByIdAsync(userId);
                if (customer?.Membership != null)
                {
                    var addonDiscounts = await membershipService.GetAddonDiscountsAsync(customer.Membership.Id);
                    customer.Membership.AddonDiscounts = addonDiscounts.ToList();
                }
                return customer ?? new Customer { Id = userId };
            }
            catch
            {
                return new Customer { Id = userId };
            }
        }

        // GET: Booking/Book?flightId=5&passengers=2
        public async Task<IActionResult> Book(int flightId, int passengers = 1)
        {
            Flight? flight = await flightSearchService.GetFlightByIdAsync(flightId);
            if (flight == null)
            {
                return NotFound();
            }

            int? userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("ChooseRole", "Auth");
            }

            List<string> occupiedSeats = await bookingService.GetOccupiedSeatsAsync(flightId);
            List<AddOn> availableAddOns = await bookingService.GetAvailableAddOnsAsync();

            int capacity = flight.Route?.Capacity ?? DefaultFlightCapacity;
            int availableSeats = capacity - occupiedSeats.Count;
            int maxPassengers = Math.Max(1, Math.Min(availableSeats, 9));
            int initialCount = Math.Max(1, Math.Min(passengers, maxPassengers));

            var (seatMapLayout, seatMapRowCount) = await bookingService.BuildSeatMapLayoutAsync(capacity);

            float basePrice = await pricingService.CalculateBasePriceAsync(flight);

            Customer customer = await GetCustomerWithMembershipAsync(userId.Value);
            float flightDiscountPct = customer.Membership?.FlightDiscountPercentage ?? 0f;
            var addonDiscountMap = new Dictionary<int, float>();
            if (customer.Membership?.AddonDiscounts != null)
            {
                foreach (var d in customer.Membership.AddonDiscounts)
                {
                    if (d.AddOn != null)
                    {
                        addonDiscountMap[d.AddOn.Id] = d.DiscountPercentage;
                    }
                }
            }

            var form = new BookingFormModel
            {
                FlightId = flightId,
                RequestedPassengers = passengers,
                Passengers = Enumerable.Range(0, initialCount)
                    .Select(_ => new PassengerInputModel())
                    .ToList()
            };

            var viewModel = new BookingPageViewModel
            {
                Flight = flight,
                BasePrice = basePrice,
                AvailableAddOns = availableAddOns,
                OccupiedSeats = occupiedSeats,
                SeatMapLayout = seatMapLayout,
                SeatMapRowCount = seatMapRowCount,
                MaximumPassengers = maxPassengers,
                Form = form,
                FlightDiscountPercentage = flightDiscountPct,
                AddonDiscountPercentages = addonDiscountMap
            };

            return View(viewModel);
        }

        // POST: Booking/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(BookingFormModel form)
        {
            int? userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("ChooseRole", "Auth");
            }

            Flight? flight = await flightSearchService.GetFlightByIdAsync(form.FlightId);
            if (flight == null)
            {
                return NotFound();
            }

            // Map form passengers to PassengerData
            var passengerDataList = new List<PassengerData>();
            foreach (var passengerInput in form.Passengers)
            {
                List<AddOn> selectedAddOns = passengerInput.SelectedAddOnIds.Any()
                    ? await bookingService.GetAddOnsByIdsAsync(passengerInput.SelectedAddOnIds)
                    : new List<AddOn>();

                passengerDataList.Add(new PassengerData
                {
                    FirstName = passengerInput.FirstName,
                    LastName = passengerInput.LastName,
                    Email = passengerInput.Email,
                    Phone = passengerInput.Phone ?? string.Empty,
                    SelectedSeat = passengerInput.SelectedSeat ?? string.Empty,
                    SelectedAddOns = selectedAddOns
                });
            }

            string validationMessage = await bookingService.ValidatePassengersAsync(passengerDataList);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                return await RedisplayBookingPage(form, flight, validationMessage);
            }

            Customer customer = await GetCustomerWithMembershipAsync(userId.Value);

            float basePrice = await pricingService.CalculateBasePriceAsync(flight);
            List<FlightTicket> tickets = bookingService.CreateTickets(flight, customer, passengerDataList, basePrice);

            foreach (FlightTicket ticket in tickets)
            {
                ticket.Price = await pricingService.CalculateTotalPriceAsync(ticket);
            }

            bool success = await bookingService.SaveTicketsAsync(tickets);
            if (!success)
            {
                return await RedisplayBookingPage(form, flight, "Booking could not be saved. Please try again.");
            }

            float totalPaid = tickets.Sum(ticket => ticket.Price);

            TempData["ConfirmedFlightNumber"] = flight.FlightNumber;
            TempData["ConfirmedFlightDate"] = flight.Date.ToString("g");
            TempData["ConfirmedTicketCount"] = tickets.Count;
            TempData["ConfirmedTotalPaid"] = totalPaid.ToString("0.00");

            return RedirectToAction(nameof(Confirmed));
        }

        // GET: Booking/Confirmed
        public IActionResult Confirmed()
        {
            if (TempData["ConfirmedFlightNumber"] == null)
            {
                return RedirectToAction("Search", "FlightTickets");
            }

            ViewBag.FlightNumber = TempData["ConfirmedFlightNumber"];
            ViewBag.FlightDate = TempData["ConfirmedFlightDate"];
            ViewBag.TicketCount = TempData["ConfirmedTicketCount"];
            ViewBag.TotalPaid = TempData["ConfirmedTotalPaid"];

            return View();
        }

        private async Task<IActionResult> RedisplayBookingPage(BookingFormModel form, Flight flight, string validationMessage)
        {
            List<string> occupiedSeats = await bookingService.GetOccupiedSeatsAsync(form.FlightId);
            List<AddOn> availableAddOns = await bookingService.GetAvailableAddOnsAsync();
            int capacity = flight.Route?.Capacity ?? DefaultFlightCapacity;
            int availableSeats = capacity - occupiedSeats.Count;
            int maxPassengers = Math.Max(1, Math.Min(availableSeats, 9));
            var (seatMapLayout, seatMapRowCount) = await bookingService.BuildSeatMapLayoutAsync(capacity);
            float basePrice = await pricingService.CalculateBasePriceAsync(flight);

            int? userId = GetCurrentUserId();
            float flightDiscountPct = 0f;
            var addonDiscountMap = new Dictionary<int, float>();
            if (userId.HasValue)
            {
                Customer customer = await GetCustomerWithMembershipAsync(userId.Value);
                flightDiscountPct = customer.Membership?.FlightDiscountPercentage ?? 0f;
                if (customer.Membership?.AddonDiscounts != null)
                {
                    foreach (var d in customer.Membership.AddonDiscounts)
                    {
                        if (d.AddOn != null)
                        {
                            addonDiscountMap[d.AddOn.Id] = d.DiscountPercentage;
                        }
                    }
                }
            }

            var viewModel = new BookingPageViewModel
            {
                Flight = flight,
                BasePrice = basePrice,
                AvailableAddOns = availableAddOns,
                OccupiedSeats = occupiedSeats,
                SeatMapLayout = seatMapLayout,
                SeatMapRowCount = seatMapRowCount,
                MaximumPassengers = maxPassengers,
                Form = form,
                ValidationMessage = validationMessage,
                FlightDiscountPercentage = flightDiscountPct,
                AddonDiscountPercentages = addonDiscountMap
            };

            return View("Book", viewModel);
        }
    }
}

