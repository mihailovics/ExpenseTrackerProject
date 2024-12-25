using ExpenseTracker.Data;
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

        [HttpGet("Income/GetAllIncomes")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetAllIncomes() 
        {
            List<Income> AllIncomes = new List<Income>();

            var user = _userManager.GetUserId(User);
            AllIncomes = await dBContext.Incomes.Where(i => i.UserId == user).ToListAsync();
           
            return View(AllIncomes);
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
        //Zasto ne radi sa FromBody
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
