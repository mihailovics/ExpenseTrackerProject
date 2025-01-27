using Elfie.Serialization;
using ExpenseTracker.Data;
using ExpenseTracker.Helpers;
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
        private readonly ICommonMethods _commonMethods;
        public ExpenseController(ILogger<HomeController> logger, ApplicationDBContext DbContext, IExpenseService ExpenseService, ICommonMethods commonMethods)
        {
            
            _logger = logger;
            dBContext = DbContext;
            _ExpenseService = ExpenseService;
            _commonMethods = commonMethods;
        }

        [HttpGet("Expense/GetExpenses")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetExpenses(int? year = null, int? month = null, string? source = null)
        {
            try
            {
                var sources = await _commonMethods.GetSourcesExpense(HttpContext, year, month);
                var years = await _commonMethods.GetYearsExpense(HttpContext, month, source);
                var months = await _commonMethods.GetMonthsExpense(HttpContext, year, source);

                var pagedExpenses = await _ExpenseService.GetPaginatedExpenses(HttpContext);

                ViewBag.TotalPages = pagedExpenses.TotalPages;
                ViewBag.CurrentPage = pagedExpenses.CurrentPage;
                ViewBag.ExpenseSum = pagedExpenses.ExpenseSum;
                ViewBag.PageSize = pagedExpenses.PageSize;
                ViewBag.Balance = pagedExpenses.Balance;

                ViewBag.SelectedYear = year;
                ViewBag.SelectedMonth = month;
                ViewBag.SelectedSource = source;

                ViewBag.Sources = sources;
                ViewBag.Years = years;
                ViewBag.Months = months;

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
