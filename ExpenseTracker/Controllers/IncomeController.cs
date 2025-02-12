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
        private readonly ICommonMethods _commonMethods;
        private readonly UserManager<User> _userManager;    

        public IncomeController(ILogger<HomeController> logger, IIncomeService incomeService, ICommonMethods commonMethods, UserManager<User> userManager)
        {
            _logger = logger;
            _incomeService = incomeService;
            _commonMethods = commonMethods;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Index(int? year = null, int? month = null, int? source = null, int? pageNumber = null, int? pageSize = null) 
        {
            var userId = _userManager.GetUserId(User);

            PaginationViewModel pagedIncomes = await _incomeService.GetPaginatedIncomes(year, month, source, pageNumber, pageSize);

            return View(pagedIncomes);
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> NewIncome()
        {
            // Ovo mora biti u service-u
            var viewModel = new ViewModel
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
        public async Task<IActionResult> NewIncome(ViewModel incomeModel)
        {
            var userId = _userManager.GetUserId(User);
            
            if (ModelState.IsValid)
            {
                var newIncome = await _incomeService.NewIncome(userId, incomeModel);

                return RedirectToAction("Index");
            }
            
            return View(incomeModel);
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Edit(int id)
        {
            //Napraviti metodu getIncomeViewModelById da ne bi pisalo toliko koda
            var income = await _incomeService.FindByid(id);
            var viewModel = new ViewModel
            {
                Amount = income.IncomeAmount,
                Description = income.Description,
                AccountId = income.AccountId,
                SourceId = income.SourceId,
                CreatedAt = income.CreatedAt,
                Account = income.Account,
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
        public async Task<IActionResult> EditIncome([FromForm] ViewModel incomeModel, [FromForm] int id)
        {
            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                var newIncome = await _incomeService.EditIncome(userId, incomeModel, id);

                if (newIncome == true)
                {
                    return RedirectToAction("Index");
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
                var deleted = await _incomeService.DeleteIncome(id);
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
                return RedirectToAction("Delete/" + id);
            }
        }
    }
}
