using System.Linq;
using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration;
using NuGet.Packaging.Signing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ExpenseTracker.Controllers
{
    public class IncomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IIncomeService _incomeService;
        private readonly ICommonMethods commonMethods;
        private readonly UserManager<User> _userManager;    

        public IncomeController(ILogger<HomeController> logger, IIncomeService incomeService, ICommonMethods CommonMethods, UserManager<User> userManager)
        {
            _logger = logger;
            _incomeService = incomeService;
            commonMethods = CommonMethods;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Index(int? year = null, int? month = null, int? source = null) 
        {
            var userId = _userManager.GetUserId(User);

            var sources = await commonMethods.GetSources("income",userId, year, month);
            var years = await commonMethods.GetYears("income",userId, month, source);
            var months = await commonMethods.GetMonths("income",userId, year, source);

            var pagedIncomes = await _incomeService.GetPaginatedIncomes(HttpContext);

            ViewBag.TotalPages = pagedIncomes.TotalPages;
            ViewBag.CurrentPage = pagedIncomes.CurrentPage;
            ViewBag.IncomeSum = pagedIncomes.IncomeSum;
            ViewBag.PageSize = pagedIncomes.PageSize;
            ViewBag.Balance = pagedIncomes.Balance;


            ViewBag.SelectedYear = year;
            ViewBag.SelectedMonth = month;
            ViewBag.SelectedSource = source;

            ViewBag.Sources = sources;
            ViewBag.Years = years;
            ViewBag.Months = months;

            return View(pagedIncomes.Incomes);
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public IActionResult NewIncome()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> NewIncome(Income incomeModel)
        {
            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                var newIncome = await _incomeService.NewIncome(userId, incomeModel);

                return RedirectToAction("GetIncomes");
            }
            
            return View(incomeModel);
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Edit(int id)
        {
            var income = await _incomeService.FindByid(id);
            return View(income);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> EditIncome([FromForm] Income incomeModel, [FromForm] int id)
        {
            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                var newIncome = await _incomeService.EditIncome(userId, incomeModel, id);

                if (newIncome == true)
                {
                    return RedirectToAction("GetIncomes");
                }
            }
            return RedirectToAction("Edit/" + id);
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Delete(int id)
        {
            var income = await _incomeService.FindByid(id);
            return View(income);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteIncome([FromForm]int id) 
        {
            
            if (ModelState.IsValid)
            {
                await _incomeService.DeleteIncome(id);
                return RedirectToAction("GetIncomes");
            }
            else
            {
                return RedirectToAction("Delete/" + id);
            }
            

        }
    }
}
