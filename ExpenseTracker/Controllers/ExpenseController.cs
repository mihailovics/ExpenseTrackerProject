using System.Drawing.Printing;
using Elfie.Serialization;
using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
using ExpenseTracker.Services.Interfaces;
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

            PaginationViewModel pagedExpenses = await _expenseService.GetPaginatedExpenses(year, month, source, pageNumber, pageSize);

            return View(pagedExpenses);
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> NewExpense()
        {
            var viewModel = await _expenseService.NewExpenseView();

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> NewExpense(GeneralViewModel expenseModel)
        {
            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                var newIncome = await _expenseService.NewExpense(userId, expenseModel);
                if (newIncome == true)
                {
                    RedirectToAction("Index");
                }
                else
                {
                    //Promeniti poruku malo je nejasna
                    TempData["ErrorMessage"] = "Insufficient balance + allowed minus on account for creating that Expense";
                    return RedirectToAction("NewExpense");
                }

                return RedirectToAction("Index");
            }

            return View(expenseModel);
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Edit(int id)
        { 
            var expense = await _expenseService.FindByid(id);

            var viewModel = await _commonMethods.ConvertExpenseToGeneral(expense);

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> EditExpense([FromForm] GeneralViewModel ExpenseModel, [FromForm] int id)
        {
            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                var newExpense = await _expenseService.EditExpense(userId, ExpenseModel, id);
 
                if (newExpense == true)
                {
                    return RedirectToAction("Index");
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
                var deleted = await _expenseService.DeleteExpense(id);
                if (deleted == true)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Delete/" + id);
                }
            }
            else
            {
                return RedirectToAction("Delete", new { id = id });
            }
        }
    }
}
