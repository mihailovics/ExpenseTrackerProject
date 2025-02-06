using System.Drawing.Printing;
using Elfie.Serialization;
using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // Smisliti nacin kako da pamtim allowedMinus i da ga vracam tamo umesto sve u balance
        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Index(int? year = null, int? month = null, int? source = null, int? pageNumber = null, int? pageSize = null)
        {
            var userId = _userManager.GetUserId(User);

            var pagedIncomes = await _expenseService.GetPaginatedExpenses(year, month, source, pageNumber, pageSize);

            PaginationViewModel model = new PaginationViewModel
            {
                TotalPages = pagedIncomes.TotalPages,
                CurrentPage = pagedIncomes.CurrentPage,
                PageSize = pagedIncomes.PageSize,
                Balance = pagedIncomes.Balance,
                Sum = pagedIncomes.Sum,
                Expenses = pagedIncomes.Expenses,
                SelectedMonth = month,
                SelectedSource = source,
                SelectedYear = year,
                Months = pagedIncomes.Months,
                Sources = pagedIncomes.Sources,
                Years = pagedIncomes.Years,
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> NewExpense()
        {
            var viewModel = new IncomeViewModel
            {
                Sources = (await _commonMethods.ShowSources())
                 .Select(s => new SelectListItem
                 {
                     Value = s.Id.ToString(),
                     Text = s.Name
                 }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> NewExpense(IncomeViewModel expenseModel)
        {
            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                var newIncome = await _expenseService.NewExpense(userId, expenseModel);

                return RedirectToAction("Index");
            }

            return View(expenseModel);
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
