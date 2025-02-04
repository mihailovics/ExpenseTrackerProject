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
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index(int? year = null, int? month = null, int? source = null, int? pageNumber = null, int? pageSize = null) 
        {
            var userId = _userManager.GetUserId(User);

            var pagedIncomes = await _incomeService.GetPaginatedIncomes(year, month, source, pageNumber, pageSize);

            PaginationViewModel model = new PaginationViewModel
            {
                TotalPages = pagedIncomes.TotalPages,
                CurrentPage = pagedIncomes.CurrentPage,
                PageSize = pagedIncomes.PageSize,
                Balance = pagedIncomes.Balance,
                IncomeSum = pagedIncomes.IncomeSum,
                Incomes = pagedIncomes.Incomes,
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
        public async Task<IActionResult> NewIncome()
        {
            var viewModel = new IncomeViewModel
            {
                Sources = (await commonMethods.ShowSources())
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
        public async Task<IActionResult> NewIncome(IncomeViewModel incomeModel)
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
