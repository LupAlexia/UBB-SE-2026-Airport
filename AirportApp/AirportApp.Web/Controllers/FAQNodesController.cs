using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Authorization;

namespace AirportApp.Web.Controllers
{
    [Authorize]
    public class FAQNodesController : Controller
    {
        private readonly IDecisionTreeService decisionTreeService;

        public FAQNodesController(IDecisionTreeService decisionTreeService)
        {
            this.decisionTreeService = decisionTreeService;
        }

        // GET: FAQNodes
        public async Task<IActionResult> Index()
        {
            return View(await decisionTreeService.GetAllNodesAsync());
        }

        // GET: FAQNodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fAQNode = await decisionTreeService.GetNodeByIdAsync((int)id);
            if (fAQNode == null)
            {
                return NotFound();
            }

            return View(fAQNode);
        }

        // GET: FAQNodes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FAQNodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NodeId,QuestionText,IsFinalAnswer")] FAQNode fAQNode)
        {
            if (ModelState.IsValid)
            {
                await decisionTreeService.CreateNodeAsync(fAQNode);
                return RedirectToAction(nameof(Index));
            }
            return View(fAQNode);
        }

        // GET: FAQNodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fAQNode = await decisionTreeService.GetNodeByIdAsync((int)id);
            if (fAQNode == null)
            {
                return NotFound();
            }
            return View(fAQNode);
        }

        // POST: FAQNodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NodeId,QuestionText,IsFinalAnswer")] FAQNode fAQNode)
        {
            if (id != fAQNode.NodeId)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(FAQNode.Options));

            if (ModelState.IsValid)
            {
                var existingNode = await decisionTreeService.GetNodeByIdAsync(id);
                if (existingNode == null)
                {
                    return NotFound();
                }

                existingNode.QuestionText = fAQNode.QuestionText;
                existingNode.IsFinalAnswer = fAQNode.IsFinalAnswer;

                await decisionTreeService.UpdateNodeAsync(id, existingNode);
                return RedirectToAction(nameof(Index));
            }
            return View(fAQNode);
        }

        // GET: FAQNodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fAQNode = await decisionTreeService.GetNodeByIdAsync((int)id);
            if (fAQNode == null)
            {
                return NotFound();
            }

            return View(fAQNode);
        }

        // POST: FAQNodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fAQNode = await decisionTreeService.GetNodeByIdAsync(id);
            if (fAQNode != null)
            {
                await decisionTreeService.DeleteNodeAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> FAQNodeExists(int id)
        {
            return decisionTreeService.GetNodeByIdAsync(id) != null;
        }
    }
}

