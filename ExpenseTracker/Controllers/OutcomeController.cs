using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    public class OutcomeController : Controller
    {
        ApplicationDBContext dBContext;
        private readonly ILogger<HomeController> _logger;
        private readonly IOutcomeService _outcomeService;

        public OutcomeController(ILogger<HomeController> logger, ApplicationDBContext DbContext, IOutcomeService outcomeService)
        {
            
            _logger = logger;
            dBContext = DbContext;
            _outcomeService = outcomeService;
        }

        [HttpGet("Outcome/GetOutcomes")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetOutcomes()
        {
            try
            {
                var pagedOutcomes = await _outcomeService.GetPaginatedOutcomes(HttpContext);

                ViewBag.Balance = pagedOutcomes.Balance;
                ViewBag.OutcomeSum = pagedOutcomes.OutcomeSum;
                ViewBag.TotalPages = pagedOutcomes.TotalPages;
                ViewBag.CurrentPage = pagedOutcomes.CurrentPage;

                return View(pagedOutcomes.Outcomes);
            }
            catch(Exception ex)
            {
                return View(ex);
            }
        }

        [HttpGet("Outcome/GetAllOutcomes")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetAllOutcomes()
        {
            var AllOutcomes = await _outcomeService.GetAllOutcomes(HttpContext);

            ViewBag.OutcomeSum = AllOutcomes.OutcomeSum;

            return View(AllOutcomes.Outcomes);
        }


        [HttpGet("Outcome/NewOutcome")]
        [Authorize(Roles = "user")]
        public IActionResult NewOutcome()
        {
            return View();
        }

        [HttpPost("Outcome/NewOutcome")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> NewOutcome(Outcome outcomeModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newOutcome = await _outcomeService.NewOutcome(HttpContext, outcomeModel);

                    return RedirectToAction("GetAllOutcomes");
                }
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Insufficient balance + allowed minus on account for creating that outcome";
                return RedirectToAction("NewOutcome");
            }

            return View(outcomeModel);

        }

        [HttpGet("Outcome/Edit/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Edit(int id)
        {
            
            var outcome = await dBContext.Outcomes.FindAsync(id);
            return View(outcome);
        }

        [HttpPost("Outcome/EditOutcome")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> EditOutcome([FromForm] Outcome outcomeModel, [FromForm] int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newOutcome = await _outcomeService.EditOutcome(HttpContext, outcomeModel, id);

                    if (newOutcome == true)
                    {
                        return RedirectToAction("GetAllOutcomes");
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Insufficient balance + allowed minus on account when editing this outcome";
                return RedirectToAction("Edit",new { id = id });
            }

            return RedirectToAction("GetAllOutcomes");
        }

        [HttpGet("Outcome/Delete/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Delete(int id)
        {

            var outcome = await dBContext.Outcomes.FindAsync(id);
            return View(outcome);
        }

        [HttpPost("Outcome/DeleteOutcome")]
        [Authorize(Roles = "user")]
        // Zasto ne radi sa FromBody
        public async Task<IActionResult> DeleteOutcome([FromForm] int id)
        {
            
            if (ModelState.IsValid)
            {
                await _outcomeService.DeleteOutcome(id);
                return RedirectToAction("GetOutcomes");
            }
            else
            {
                return RedirectToAction("GetOutcomes");
            }


        }
    }
}
