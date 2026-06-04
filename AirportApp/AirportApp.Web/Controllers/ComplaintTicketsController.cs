using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Web.Models.ComplaintTicket;

namespace AirportApp.Web.Controllers
{
    [Authorize]
    public class ComplaintTicketsController : Controller
    {
        private readonly IComplaintTicketService complaintTicketService;
        private readonly IUserService userService;
        private readonly IComplaintTicketCategoryService complaintTicketCategoryService;
        private readonly IComplaintTicketSubcategoryService complaintTicketSubcategoryService;

        public ComplaintTicketsController(
            IComplaintTicketService complaintTicketService,
            IUserService userService,
            IComplaintTicketCategoryService complaintTicketCategoryService,
            IComplaintTicketSubcategoryService complaintTicketSubcategoryService)
        {
            this.complaintTicketService = complaintTicketService;
            this.userService = userService;
            this.complaintTicketCategoryService = complaintTicketCategoryService;
            this.complaintTicketSubcategoryService = complaintTicketSubcategoryService;
        }

        // GET: ComplaintTickets
        public async Task<IActionResult> Index(TicketFilterStatusEnum status = TicketFilterStatusEnum.ALL)
        {
            IEnumerable<ComplaintTicket> tickets = await complaintTicketService.GetAllTicketsAsync();

            if (User.IsInRole("Customer"))
            {
                User? currentUser = await ResolveCurrentUserAsync();
                if (currentUser != null)
                {
                    tickets = tickets.Where(ticket => ticket.Creator?.Id == currentUser.Id);
                }

                await PopulateCreateOptionsAsync();
            }

            if (status != TicketFilterStatusEnum.ALL)
            {
                tickets = tickets.Where(ticket => ticket.CurrentStatus.ToString() == status.ToString());
            }

            ViewBag.SelectedStatus = status;
            return View(tickets.OrderByDescending(ticket => ticket.CreationTimestamp));
        }

        // GET: ComplaintTickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complaintTicket = await complaintTicketService.GetTicketByIdAsync((int)id);
            if (complaintTicket == null)
            {
                return NotFound();
            }

            return View(complaintTicket);
        }

        // GET: ComplaintTickets/Create
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create()
        {
            await PopulateCreateOptionsAsync();
            return View(new CreateComplaintTicketViewModel());
        }

        // POST: ComplaintTickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateComplaintTicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User? creator = await ResolveCurrentUserAsync();
                    if (creator == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to resolve the current user as a ticket creator.");
                    }
                    else
                    {
                        ComplaintTicketCategory category = await complaintTicketCategoryService.GetCategoryByIdAsync(model.CategoryId);
                        ComplaintTicketSubcategory subcategory = await complaintTicketSubcategoryService.GetSubcategoryByIdAsync(model.SubcategoryId);

                        if (subcategory.ParentCategory.Id != category.Id)
                        {
                            ModelState.AddModelError(nameof(model.SubcategoryId), "The selected subcategory does not belong to the selected category.");
                        }
                        else
                        {
                            var complaintTicket = new ComplaintTicket
                            {
                                Creator = creator,
                                Category = category,
                                Subcategory = subcategory,
                                Subject = model.Subject,
                                Description = model.Description,
                                CreationTimestamp = DateTime.UtcNow,
                                CurrentStatus = ComplaintTicketStatusEnum.OPEN,
                                UrgencyLevel = model.UrgencyLevel ?? category.CategoryUrgencyLevel
                            };

                            await complaintTicketService.AddTicketAsync(complaintTicket);
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
                catch (KeyNotFoundException)
                {
                    ModelState.AddModelError(string.Empty, "The selected category or subcategory was not found.");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            await PopulateCreateOptionsAsync(model.CategoryId, model.SubcategoryId);
            return View(model);
        }

        // GET: ComplaintTickets/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complaintTicket = await complaintTicketService.GetTicketByIdAsync((int)id);
            if (complaintTicket == null)
            {
                return NotFound();
            }
            return View(complaintTicket);
        }

        // POST: ComplaintTickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UrgencyLevel,CurrentStatus")] ComplaintTicket complaintTicket)
        {
            if (id != complaintTicket.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Subject");
            ModelState.Remove("Description");
            ModelState.Remove("Creator");
            ModelState.Remove("Category");
            ModelState.Remove("Subcategory");

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await ComplaintTicketExists(id))
                    {
                        return NotFound();
                    }

                    await complaintTicketService.UpdateUrgencyLevelAsync(id, complaintTicket.UrgencyLevel);
                    await complaintTicketService.UpdateStatusAsync(id, complaintTicket.CurrentStatus);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error saving in API: {ex.Message}");
                }
            }

            return View(complaintTicket);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, ComplaintTicketStatusEnum currentStatus, TicketFilterStatusEnum status = TicketFilterStatusEnum.ALL)
        {
            if (!await ComplaintTicketExists(id))
            {
                return NotFound();
            }

            await complaintTicketService.UpdateStatusAsync(id, currentStatus);
            return RedirectToAction(nameof(Index), new { status });
        }

        // GET: ComplaintTickets/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complaintTicket = await complaintTicketService.GetTicketByIdAsync((int)id);
            if (complaintTicket == null)
            {
                return NotFound();
            }

            return View(complaintTicket);
        }

        // POST: ComplaintTickets/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var complaintTicket = await complaintTicketService.GetTicketByIdAsync(id);
            if (complaintTicket != null)
            {
                await complaintTicketService.DeleteTicketByIdAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ComplaintTicketExists(int id)
        {
            return await complaintTicketService.GetTicketByIdAsync(id) != null;
        }

        private async Task<User?> ResolveCurrentUserAsync()
        {
            string? claimUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claimUserId, out int parsedUserId))
            {
                try
                {
                    return await userService.GetByIdAsync(parsedUserId);
                }
                catch (KeyNotFoundException)
                {
                }
            }

            if (UserSession.CurrentUser?.Id is int sessionUserId && sessionUserId > 0)
            {
                try
                {
                    return await userService.GetByIdAsync(sessionUserId);
                }
                catch (KeyNotFoundException)
                {
                }
            }

            string? email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                email = UserSession.CurrentUser?.Email;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            List<User> users = await userService.GetAllUsersAsync();
            return users.FirstOrDefault(user =>
                string.Equals(user.EmailAddress, email, StringComparison.OrdinalIgnoreCase));
        }

        private async Task PopulateCreateOptionsAsync(int? selectedCategoryId = null, int? selectedSubcategoryId = null)
        {
            List<ComplaintTicketCategory> categories = (await complaintTicketCategoryService.GetAllCategoriesAsync()).ToList();
            Dictionary<int, List<ComplaintTicketSubcategory>> subcategoriesByCategory = new();
            foreach (ComplaintTicketCategory category in categories)
            {
                subcategoriesByCategory[category.Id] = (await complaintTicketSubcategoryService.GetSubcategoriesByCategoryIdAsync(category.Id)).ToList();
            }

            int? resolvedCategoryId = selectedCategoryId
                ?? categories.FirstOrDefault(category => subcategoriesByCategory[category.Id].Count > 0)?.Id
                ?? categories.FirstOrDefault()?.Id;
            List<SelectListItem> categoryItems = categories
                .Select(category => new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName,
                    Selected = resolvedCategoryId.HasValue && resolvedCategoryId.Value == category.Id
                })
                .ToList();

            List<SelectListItem> subcategoryItems = new List<SelectListItem>();
            List<object> subcategoryDetails = new List<object>();
            foreach (ComplaintTicketCategory category in categories)
            {
                IEnumerable<ComplaintTicketSubcategory> subcategories = subcategoriesByCategory[category.Id];
                subcategoryItems.AddRange(subcategories.Select(subcategory => new SelectListItem
                {
                    Value = subcategory.Id.ToString(),
                    Text = $"{category.CategoryName} - {subcategory.SubcategoryName}",
                    Selected = selectedSubcategoryId.HasValue && selectedSubcategoryId.Value == subcategory.Id
                }));
                subcategoryDetails.AddRange(subcategories.Select(subcategory => new
                {
                    Id = subcategory.Id,
                    Name = subcategory.SubcategoryName,
                    CategoryId = category.Id
                }));
            }

            ViewBag.Categories = categoryItems;
            ViewBag.Subcategories = subcategoryItems;
            ViewBag.SubcategoryDetails = subcategoryDetails;
            ViewBag.SelectedCategoryId = resolvedCategoryId;
            ViewBag.InitialSubcategories = resolvedCategoryId.HasValue
                ? subcategoriesByCategory[resolvedCategoryId.Value]
                    .Select(subcategory => new SelectListItem
                    {
                        Value = subcategory.Id.ToString(),
                        Text = categories.First(category => category.Id == resolvedCategoryId.Value).CategoryName + " - " + subcategory.SubcategoryName,
                        Selected = selectedSubcategoryId.HasValue && selectedSubcategoryId.Value == subcategory.Id
                    })
                    .ToList()
                : new List<SelectListItem>();
        }
    }
}

