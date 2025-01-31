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
        private readonly ILogger<HomeController> _logger;
        private readonly IExpenseService _expenseService;
        private readonly UserManager<User> _userManager;
        private readonly ICommonMethods _commonMethods;

        public ExpenseController(ILogger<HomeController> logger, IExpenseService ExpenseService, ICommonMethods commonMethods, UserManager<User> userManager)
        {
            _logger = logger;
            _expenseService = ExpenseService;
            _commonMethods = commonMethods;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetExpenses(int? year = null, int? month = null, string? source = null)
        {
            var userId = _userManager.GetUserId(User);

            var sources = await _commonMethods.GetSources("expense",userId, year, month);
            var years = await _commonMethods.GetYears("expense",userId, month, source);
            var months = await _commonMethods.GetMonths("expense",userId, year, source);

            var pagedExpenses = await _expenseService.GetPaginatedExpenses(HttpContext);
            // ViewModel napraviti umesto 
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

        [HttpGet]
        [Authorize(Roles = "user")]
        public IActionResult NewExpense()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> NewExpense(Expense ExpenseModel)
        {
            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                var newExpense = await _expenseService.NewExpense(userId, ExpenseModel);
                if (newExpense == true)
                {
                    return RedirectToAction("GetExpenses");
                }
                else
                {
                    TempData["ErrorMessage"] = "Insufficient balance + allowed minus on account for creating that Expense";
                    return RedirectToAction("NewExpense");
                }
            }
            return RedirectToAction("GetExpenses");
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Edit(int id)
        { 
            var expense = await _expenseService.FindByid(id);
            return View(expense);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> EditExpense([FromForm] Expense ExpenseModel, [FromForm] int id)
        {
            var userId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                var newExpense = await _expenseService.EditExpense(userId, ExpenseModel, id);
 
                if (newExpense == true)
                {
                    return RedirectToAction("GetExpenses");
                }
                else
                {
                    TempData["ErrorMessage"] = "Insufficient balance + allowed minus on account when editing this Expense";
                    return RedirectToAction("Edit", new { id = id });
                }
            }

            return RedirectToAction("GetExpenses");
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _expenseService.FindByid(id);
            return View(expense);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteExpense([FromForm] int id)
        {
            if (ModelState.IsValid)
            {
                await _expenseService.DeleteExpense(id);
                return RedirectToAction("GetExpenses");
            }
            else
            {
                return RedirectToAction("Delete", new { id = id });
            }
        }
    }
}
