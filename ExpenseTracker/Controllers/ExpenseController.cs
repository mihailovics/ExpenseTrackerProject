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
    public class ExpenseController : Controller
    {
        ApplicationDBContext dBContext;
        private readonly ILogger<HomeController> _logger;
        private readonly IExpenseService _ExpenseService;

        public ExpenseController(ILogger<HomeController> logger, ApplicationDBContext DbContext, IExpenseService ExpenseService)
        {
            
            _logger = logger;
            dBContext = DbContext;
            _ExpenseService = ExpenseService;
        }

        [HttpGet("Expense/GetExpenses")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetExpenses()
        {
            try
            {
                var pagedExpenses = await _ExpenseService.GetPaginatedExpenses(HttpContext);

                ViewBag.Balance = pagedExpenses.Balance;
                ViewBag.ExpenseSum = pagedExpenses.ExpenseSum;
                ViewBag.TotalPages = pagedExpenses.TotalPages;
                ViewBag.CurrentPage = pagedExpenses.CurrentPage;

                return View(pagedExpenses.Expenses);
            }
            catch(Exception ex)
            {
                return View(ex);
            }
        }

        [HttpGet("Expense/GetAllExpenses")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetAllExpenses()
        {
            var AllExpenses = await _ExpenseService.GetAllExpenses(HttpContext);

            ViewBag.ExpenseSum = AllExpenses.ExpenseSum;

            return View(AllExpenses.Expenses);
        }


        [HttpGet("Expense/NewExpense")]
        [Authorize(Roles = "user")]
        public IActionResult NewExpense()
        {
            return View();
        }

        [HttpPost("Expense/NewExpense")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> NewExpense(Expense ExpenseModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newExpense = await _ExpenseService.NewExpense(HttpContext, ExpenseModel);

                    return RedirectToAction("GetAllExpenses");
                }
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Insufficient balance + allowed minus on account for creating that Expense";
                return RedirectToAction("NewExpense");
            }

            return View(ExpenseModel);

        }

        [HttpGet("Expense/Edit/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Edit(int id)
        {
            
            var Expense = await dBContext.Expenses.FindAsync(id);
            return View(Expense);
        }

        [HttpPost("Expense/EditExpense")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> EditExpense([FromForm] Expense ExpenseModel, [FromForm] int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newExpense = await _ExpenseService.EditExpense(HttpContext, ExpenseModel, id);

                    if (newExpense == true)
                    {
                        return RedirectToAction("GetAllExpenses");
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Insufficient balance + allowed minus on account when editing this Expense";
                return RedirectToAction("Edit",new { id = id });
            }

            return RedirectToAction("GetAllExpenses");
        }

        [HttpGet("Expense/Delete/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Delete(int id)
        {

            var Expense = await dBContext.Expenses.FindAsync(id);
            return View(Expense);
        }

        [HttpPost("Expense/DeleteExpense")]
        [Authorize(Roles = "user")]
        // Zasto ne radi sa FromBody
        public async Task<IActionResult> DeleteExpense([FromForm] int id)
        {
            
            if (ModelState.IsValid)
            {
                await _ExpenseService.DeleteExpense(id);
                return RedirectToAction("GetExpenses");
            }
            else
            {
                return RedirectToAction("GetExpenses");
            }


        }
    }
}
