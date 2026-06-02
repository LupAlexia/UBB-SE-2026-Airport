using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Web.Models.FaqEntries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Web.Controllers
{
    [Authorize]
    public class FAQEntriesController : Controller
    {
        private readonly IFAQService fAQService;

        public FAQEntriesController(IFAQService fAQService)
        {
            this.fAQService = fAQService;
        }

        // GET: FAQEntries
        public async Task<IActionResult> Index(FAQCategoryEnum category = FAQCategoryEnum.All, string? searchQuery = null)
        {
            var startingFilteredFaqs = await fAQService.FilterFAQEntryAsync(category, searchQuery);
            var faqViewModel = new FaqEntriesIndexViewModel
            {
                SelectedCategory = category,
                SearchQuery = searchQuery,
                PopularFAQs = startingFilteredFaqs.OrderByDescending(f => f.ViewCount)
                    .Take(5)
                    .ToList(),
                FilteredFAQs = startingFilteredFaqs
                    .OrderByDescending(f => f.ViewCount)
                    .ToList()
            };
            return View(faqViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> OpenDetails(int id)
        {
            var all = await fAQService.GetAllAsync();
            var entry = all.FirstOrDefault(f => f.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            await fAQService.IncrementViewCountAsync(entry);
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: FAQEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var all = await fAQService.GetAllAsync();
            var fAQEntry = all.FirstOrDefault(frequentQuestion => frequentQuestion.Id == id);
            if (fAQEntry == null)
            {
                return NotFound();
            }

            return View(fAQEntry);
        }

        // GET: FAQEntries/Create
        [Authorize(Roles = "Employee")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: FAQEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Question,Answer,Category")] FAQEntry fAQEntry)
        {
            if (ModelState.IsValid)
            {
                await fAQService.AddFAQEntryAsync(fAQEntry);
                return RedirectToAction(nameof(Index));
            }
            return View(fAQEntry);
        }

        // GET: FAQEntries/Edit/5
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var all = await fAQService.GetAllAsync();
            var fAQntry = all.FirstOrDefault(frequentQuestion => frequentQuestion.Id == id);
            if (fAQntry == null)
            {
                return NotFound();
            }

            return View(fAQntry);
        }

        // POST: FAQEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Question,Answer,Category,ViewCount,HelpfulVotesCount,NotHelpfulVotesCount")] FAQEntry fAQEntry)
        {
            if (id != fAQEntry.Id)
            {
                return NotFound();
            }

            // var existing = (await fAQService.GetAllAsync()).FirstOrDefault(entry => entry.Id == id);
            // if (existing == null)
            // {
            //    return NotFound();
            // }

            // fAQEntry.ViewCount = existing.ViewCount;
            // fAQEntry.HelpfulVotesCount = existing.HelpfulVotesCount;
            // fAQEntry.NotHelpfulVotesCount = existing.NotHelpfulVotesCount;
            if (ModelState.IsValid)
            {
                await fAQService.EditFAQEntryAsync(fAQEntry, id);
                return RedirectToAction(nameof(Index));
            }
            return View(fAQEntry);
        }

        // GET: FAQEntries/Delete/5
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var all = await fAQService.GetAllAsync();
            var fAQEntry = all.FirstOrDefault(frequentQuestion => frequentQuestion.Id == id);

            if (fAQEntry == null)
            {
                return NotFound();
            }

            return View(fAQEntry);
        }

        // POST: FAQEntries/Delete/5
        [Authorize(Roles = "Employee")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await fAQService.DeleteFAQEntryAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> IncrementHelpfulVotes(int id)
        {
            var all = await fAQService.GetAllAsync();
            var fAQntry = all.FirstOrDefault(frequentQuestion => frequentQuestion.Id == id);
            if (fAQntry == null)
            {
                return NotFound();
            }
            await fAQService.IncrementWasHelpfulVotesAsync(fAQntry);
            // all = await fAQService.GetAllAsync();
            // var updatedFaqEntry = all.FirstOrDefault(frequentQuestion => frequentQuestion.Id == id);
            // return View(updatedFaqEntry);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> IncrementNotHelpfulVotes(int id)
        {
            var all = await fAQService.GetAllAsync();
            var fAQntry = all.FirstOrDefault(frequentQuestion => frequentQuestion.Id == id);
            if (fAQntry == null)
            {
                return NotFound();
            }
            await fAQService.IncrementWasNotHelpfulVotesAsync(fAQntry);
            // all = await fAQService.GetAllAsync();
            // var updatedFaqEntry = all.FirstOrDefault(frequentQuestion => frequentQuestion.Id == id);
            // return View(updatedFaqEntry);
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}

