using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    public class OutcomeController : Controller
    {
        ApplicationDBContext dBContext;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;

        public OutcomeController(ILogger<HomeController> logger, UserManager<User> UserManager, ApplicationDBContext DbContext)
        {
            _userManager = UserManager;
            _logger = logger;
            dBContext = DbContext;
        }

        [HttpGet("Outcome/GetOutcomes")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetOutcomes()
        {
            List<Outcome> AllOutcomes = new List<Outcome>();
            var pageNumberString = HttpContext.Request.Query["pageNumber"].FirstOrDefault();
            int pageNumber = string.IsNullOrEmpty(pageNumberString) ? 1 : int.Parse(pageNumberString);
            int pageSize = 5;

            var user = _userManager.GetUserId(User);

            IQueryable<Outcome> query = dBContext.Outcomes.Where(i => i.UserId == user);

            int totalIncomes = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalIncomes / (double)pageSize);
            List<Outcome> pagedOutcomes = await query
                .Skip((pageNumber - 1) * pageSize) // Skip records from previous pages
                .Take(pageSize) // Take records for the current page
                .ToListAsync();

            decimal outcomeSum = pagedOutcomes.Sum(i => i.OutcomeAmount);

            ViewBag.OutcomeSum = outcomeSum;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;

            return View(pagedOutcomes);
        }

        [HttpGet("Outcome/GetAllOutcomes")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetAllOutcomes()
        {
            List<Outcome> AllOutcomes = new List<Outcome>();

            var user = _userManager.GetUserId(User);
            AllOutcomes = await dBContext.Outcomes.Where(i => i.UserId == user).ToListAsync();

            decimal outcomeSum = AllOutcomes.Sum(i => i.OutcomeAmount);

            ViewBag.OutcomeSum = outcomeSum;

            return View(AllOutcomes);
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

            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                outcomeModel.User.Name = user.Name;
                outcomeModel.User = user;
                outcomeModel.UserId = user.Id;
                outcomeModel.CreatedAt = DateTime.Now;

                dBContext.Outcomes.Add(outcomeModel);
                await dBContext.SaveChangesAsync();

                return RedirectToAction("GetAllOutcomes");
            }


            return View(outcomeModel);
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
            var outcome = await dBContext.Outcomes.FindAsync(id);
            if (ModelState.IsValid)
            {
                dBContext.Outcomes.Remove(outcome);
                await dBContext.SaveChangesAsync();
                return RedirectToAction("GetOutcomes");
            }
            else
            {
                return RedirectToAction("GetOutcomes");
            }


        }
    }
}
