using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AirportApp.Web.Controllers
{
    [Authorize]
    public class FlightTicketsController : Controller
    {
        private readonly IDashboardService dashboardService;
        private readonly IFlightSearchService flightSearchService;
        private readonly IAuthService authService;
        private readonly IPricingService pricingService;

        public FlightTicketsController(IDashboardService dashboard, IFlightSearchService flightSearch, IAuthService auth, IPricingService pricing)
        {
            dashboardService = dashboard;
            flightSearchService = flightSearch;
            authService = auth;
            pricingService = pricing;
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

        private string? GetCurrentUserName()
        {
            return User.Identity?.Name ?? User.FindFirstValue(ClaimTypes.Name);
        }

        private void SetCurrentUserViewData()
        {
            ViewBag.CurrentUserId = GetCurrentUserId();
            ViewBag.CurrentUserName = GetCurrentUserName();
        }

        private async Task<FlightTicket?> GetCurrentUsersTicketAsync(int ticketId)
        {
            int? resolvedUserId = GetCurrentUserId();
            if (!resolvedUserId.HasValue)
            {
                return null;
            }

            IEnumerable<FlightTicket> tickets = await dashboardService.GetTicketsByUserIdAsync(resolvedUserId.Value);
            return tickets.FirstOrDefault(ticket => ticket.Id == ticketId);
        }

        // GET: FlightTickets
        public async Task<IActionResult> Index(string ticketFilter = "Upcoming")
        {
            SetCurrentUserViewData();
            int? resolvedUserId = GetCurrentUserId();
            string selectedTicketFilter = string.Equals(ticketFilter, "Past", StringComparison.OrdinalIgnoreCase)
                ? "Past"
                : "Upcoming";

            ViewBag.TicketFilters = new List<string> { "Upcoming", "Past" };
            ViewBag.SelectedTicketFilter = selectedTicketFilter;

            IEnumerable<FlightTicket> tickets = resolvedUserId.HasValue
                ? await dashboardService.GetUserTicketsAsync(resolvedUserId.Value, selectedTicketFilter)
                : new List<FlightTicket>();

            return View(tickets);
        }

        // GET: FlightTickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SetCurrentUserViewData();

            FlightTicket? flightTicket = await GetCurrentUsersTicketAsync((int)id);
            if (flightTicket == null)
            {
                return NotFound();
            }

            return View(flightTicket);
        }

        // GET: FlightTickets/Create
        public IActionResult Create(int? flightId)
        {
            SetCurrentUserViewData();
            ViewBag.FlightId = flightId;
            return View();
        }

        // POST: FlightTickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? flightId, [Bind("Id,Seat,Price,Status,PassengerFirstName,PassengerLastName,PassengerEmail,PassengerPhone")] FlightTicket flightTicket)
        {
            SetCurrentUserViewData();
            int? resolvedUserId = GetCurrentUserId();
            if (!resolvedUserId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "A user id is required to create a flight ticket.");
                ViewBag.FlightId = flightId;
                return View(flightTicket);
            }

            if (!flightId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "A flight must be selected before creating a ticket.");
                ViewBag.FlightId = null;
                return View(flightTicket);
            }

            ModelState.Remove(nameof(FlightTicket.User));
            ModelState.Remove(nameof(FlightTicket.Flight));

            if (ModelState.IsValid)
            {
                var currentUser = await authService.GetByIdAsync(resolvedUserId.Value);
                flightTicket.User = currentUser ?? new Customer { Id = resolvedUserId.Value };
                flightTicket.Flight = new Flight { Id = flightId.Value };

                // Apply discount dynamically on the entered price
                flightTicket.Price = await pricingService.CalculateTotalPriceAsync(flightTicket);

                await dashboardService.AddTicketAsync(flightTicket);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.FlightId = flightId;
            return View(flightTicket);
        }

        // GET: FlightTickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SetCurrentUserViewData();

            FlightTicket? flightTicket = await GetCurrentUsersTicketAsync((int)id);
            if (flightTicket == null)
            {
                return NotFound();
            }
            return View(flightTicket);
        }

        // POST: FlightTickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status")] FlightTicket flightTicket)
        {
            SetCurrentUserViewData();
            if (id != flightTicket.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(FlightTicket.Seat));
            ModelState.Remove(nameof(FlightTicket.Price));
            ModelState.Remove(nameof(FlightTicket.PassengerFirstName));
            ModelState.Remove(nameof(FlightTicket.PassengerLastName));
            ModelState.Remove(nameof(FlightTicket.PassengerEmail));
            ModelState.Remove(nameof(FlightTicket.PassengerPhone));
            ModelState.Remove(nameof(FlightTicket.User));
            ModelState.Remove(nameof(FlightTicket.Flight));

            int? resolvedUserId = GetCurrentUserId();
            if (!resolvedUserId.HasValue)
            {
                return BadRequest("A user id is required to edit a flight ticket.");
            }

            if (ModelState.IsValid)
            {
                FlightTicket? existingTicket = await GetCurrentUsersTicketAsync(id);
                if (existingTicket == null)
                {
                    return NotFound();
                }

                await dashboardService.UpdateTicketStatusAsync(id, flightTicket.Status);
                return RedirectToAction(nameof(Index));
            }

            return View(flightTicket);
        }

        // GET: FlightTickets/Cancel/5
        public async Task<IActionResult> Cancel(int id)
        {
            FlightTicket? flightTicket = await GetCurrentUsersTicketAsync(id);
            if (flightTicket == null)
            {
                return NotFound();
            }

            await dashboardService.UpdateTicketStatusAsync(id, "Cancelled");
            return RedirectToAction(nameof(Index));
        }

        // GET: FlightTickets/DownloadPdf/5
        public async Task<IActionResult> DownloadPdf(int id)
        {
            FlightTicket? flightTicket = await GetCurrentUsersTicketAsync(id);
            if (flightTicket == null)
            {
                return NotFound();
            }

            try
            {
                QuestPDF.Settings.License = LicenseType.Community;
                byte[] fileBytes = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(textStyle => textStyle.FontSize(12));

                        page.Header()
                            .Text("WizzErr Boarding Pass")
                            .SemiBold().FontSize(28).FontColor(Colors.Blue.Darken2);

                        page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                        {
                            col.Spacing(5);
                            col.Item().Text($"FlightTicket ID: {flightTicket.Id}").FontSize(14).SemiBold();
                            col.Item().Text($"Status: {flightTicket.Status}").FontColor(flightTicket.Status == "Cancelled" ? Colors.Red.Medium : Colors.Green.Darken1);
                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            col.Item().PaddingTop(10).Text("Flight Details").FontSize(16).SemiBold();
                            col.Item().Text($"Flight Number: {flightTicket.Flight?.FlightNumber ?? "N/A"}");
                            col.Item().Text($"Date: {flightTicket.Flight?.Date:dd MMM yyyy HH:mm}");
                            col.Item().Text($"Route: {flightTicket.Flight?.Route?.Airport?.City ?? "N/A"} ({flightTicket.Flight?.Route?.RouteType ?? "N/A"})");
                            col.Item().Text($"Departure: {flightTicket.Flight?.Route?.DepartureTime:HH:mm}");
                            col.Item().Text($"Arrival: {flightTicket.Flight?.Route?.ArrivalTime:HH:mm}");
                            col.Item().Text($"Seat: {flightTicket.Seat ?? "Unassigned"}");

                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            col.Item().PaddingTop(10).Text("Passenger Information").FontSize(16).SemiBold();
                            col.Item().Text($"Name: {flightTicket.PassengerFirstName} {flightTicket.PassengerLastName}");
                            col.Item().Text($"Email: {flightTicket.PassengerEmail}");
                            col.Item().Text($"Phone: {flightTicket.PassengerPhone}");

                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            col.Item().PaddingTop(10).Text("Selected Add-Ons").FontSize(16).SemiBold();
                            if (flightTicket.SelectedAddOns != null && flightTicket.SelectedAddOns.Count > 0)
                            {
                                foreach (var addOn in flightTicket.SelectedAddOns)
                                {
                                    col.Item().Text($"â€¢ {addOn.Name}");
                                }
                            }
                            else
                            {
                                col.Item().Text("No add-ons selected");
                            }

                            col.Item().PaddingTop(15).Text($"Total Price: {flightTicket.Price} EUR").FontSize(16).SemiBold();
                        });

                        page.Footer().AlignCenter().Text(textDescriptor =>
                        {
                            textDescriptor.Span("Page ");
                            textDescriptor.CurrentPageNumber();
                            textDescriptor.Span(" of ");
                            textDescriptor.TotalPages();
                        });
                    });
                })
                .GeneratePdf();

                return File(fileBytes, "application/pdf", $"FlightTicket_{flightTicket.Id}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to generate PDF: {ex.Message}");
            }
        }

        private async Task<bool> FlightTicketExists(int id)
        {
            return await GetCurrentUsersTicketAsync(id) != null;
        }

        // GET: FlightTickets/Search
        public IActionResult Search()
        {
            SetCurrentUserViewData();
            return View();
        }

        // POST: FlightTickets/Search
        [HttpPost]
        public async Task<IActionResult> Search(string location, bool isDeparture, string date, string passengers)
        {
            SetCurrentUserViewData();
            ViewBag.Location = location;
            ViewBag.IsDeparture = isDeparture;
            ViewBag.Date = date;
            ViewBag.Passengers = passengers;

            if (string.IsNullOrEmpty(location))
            {
                ModelState.AddModelError(string.Empty, "Please enter a location.");
                return View();
            }

            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var flightDateTime))
            {
                parsedDate = flightDateTime;
            }

            int? parsedPassengers = null;
            if (!string.IsNullOrEmpty(passengers))
            {
                parsedPassengers = flightSearchService.ParsePassengerCount(passengers);
            }

            try
            {
                var flights = (await flightSearchService.SearchFlightsAsync(location, isDeparture, parsedDate, parsedPassengers)).ToList();
                var basePrices = new Dictionary<int, float>();
                foreach (var flight in flights)
                {
                    basePrices[flight.Id] = await pricingService.CalculateBasePriceAsync(flight);
                }

                ViewBag.BasePrices = basePrices;
                return View("SearchResults", flights);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error searching flights: {ex.Message}");
                return View();
            }
        }
    }
}

