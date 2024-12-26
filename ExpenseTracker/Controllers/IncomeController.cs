using System.Linq;
using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    public class IncomeController : Controller
    {
        ApplicationDBContext dBContext;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        
        public IncomeController(ILogger<HomeController> logger, UserManager<User> UserManager,ApplicationDBContext DbContext)
        {
            _userManager = UserManager;
            _logger = logger;
            dBContext = DbContext;
        }

        [HttpGet("Income/GetIncomes")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetIncomes() 
        {
            List<Income> AllIncomes = new List<Income>();
            var pageNumberString = HttpContext.Request.Query["pageNumber"].FirstOrDefault();
            int pageNumber = string.IsNullOrEmpty(pageNumberString) ? 1 : int.Parse(pageNumberString);
            int pageSize = 5;
            
            var user = _userManager.GetUserId(User);
         // var userU = await _userManager.GetUserAsync(User);

            IQueryable<Income> query = dBContext.Incomes.Where(i => i.UserId == user);

            int totalIncomes = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalIncomes / (double)pageSize);
            List<Income> pagedIncomes = await query
                .Skip((pageNumber - 1) * pageSize) 
                .Take(pageSize) 
                .ToListAsync();

            decimal incomeSum = pagedIncomes.Sum(i => i.IncomeAmount);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.IncomeSum = incomeSum;

            return View(pagedIncomes);
        }
        
        [HttpGet("Income/GetAllIncomes")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetAllIncomes()
        {
            List<Income> AllIncomes = new List<Income>();

            var user = _userManager.GetUserId(User);
            AllIncomes = await dBContext.Incomes.Where(i => i.UserId == user).ToListAsync();

            decimal incomeSum = AllIncomes.Sum(i => i.IncomeAmount);
            ViewBag.IncomeSum = incomeSum;

            return View(AllIncomes);
        }

        [HttpGet("Income/NewIncome")]
        [Authorize(Roles = "user")]
        public IActionResult NewIncome()
        {
            return View();
        }

        [HttpPost("Income/NewIncome")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> NewIncome(Income incomeModel)
        {
                // System.InvalidOperation 
                var user = await _userManager.GetUserAsync(User);
                
                if (ModelState.IsValid)
                {
                    
                    incomeModel.User.Name = user.Name;
                    incomeModel.User = user;
                    incomeModel.UserId = user.Id;
                    incomeModel.CreatedAt = DateTime.Now;

                    dBContext.Incomes.Add(incomeModel);
                    await dBContext.SaveChangesAsync();

                    return RedirectToAction("GetAllIncomes");
                }
            
            
            return View(incomeModel);
        }

        [HttpGet("Income/Delete/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Delete(int id)
        {
            
            var income = await dBContext.Incomes.FindAsync(id);
            return View(income);
        }

        [HttpPost("Income/DeleteIncome")]
        [Authorize(Roles = "user")]
        // Zasto ne radi sa FromBody
        public async Task<IActionResult> DeleteIncome([FromForm]int id) 
        {
            var income = await dBContext.Incomes.FindAsync(id);
            if (ModelState.IsValid)
            {
                dBContext.Incomes.Remove(income);
                await dBContext.SaveChangesAsync();
                return RedirectToAction("GetAllIncomes");
            }
            else
            {
                return RedirectToAction("GetAllIncomes");
            }
            

        }
    }
}
