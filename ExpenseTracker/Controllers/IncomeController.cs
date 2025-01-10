using System.Linq;
using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
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
        ApplicationDBContext dBContext;
        private readonly ILogger<HomeController> _logger;
        private readonly IIncomeService _incomeService;
        private readonly ICommonMethods commonMethods;

        public IncomeController(ILogger<HomeController> logger, ApplicationDBContext DbContext, IIncomeService incomeService, ICommonMethods CommonMethods)
        {
            
            _logger = logger;
            dBContext = DbContext;
            _incomeService = incomeService;
            commonMethods = CommonMethods;
        }

        [HttpGet("Income/GetIncomes")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetIncomes() 
        {
            try
            {
                var sources = await commonMethods.GetDistinctSourcesAsync(HttpContext);
                var years = await commonMethods.GetDistinctYearsAsync(HttpContext);
                var months = await commonMethods.GetDistinctMonthsAsync(HttpContext);

                var pagedIncomes = await _incomeService.GetPaginatedIncomes(HttpContext);

                ViewBag.TotalPages = pagedIncomes.TotalPages;
                ViewBag.CurrentPage = pagedIncomes.CurrentPage;
                ViewBag.IncomeSum = pagedIncomes.IncomeSum;
                ViewBag.PageSize = pagedIncomes.PageSize;
                ViewBag.Balance = pagedIncomes.Balance;
                //Filtering
                ViewBag.Sources = sources;
                ViewBag.Years = years;
                ViewBag.Months = months;

                return View(pagedIncomes.Incomes);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
        
        [HttpGet("Income/GetAllIncomes")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetAllIncomes()
        {
            try
            {
                var AllIncomes = await _incomeService.GetAllIncomes(HttpContext);

                ViewBag.IncomeSum = AllIncomes.IncomeSum;
                ViewBag.Balance = AllIncomes.Balance;
                return View(AllIncomes.Incomes);
            }
            catch (Exception ex) 
            {
                return View(ex);
            }
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
                
            if (ModelState.IsValid)
            {
                var newIncome = await _incomeService.NewIncome(HttpContext, incomeModel);

                return RedirectToAction("GetAllIncomes");
            }
            
            return View(incomeModel);
        }

        [HttpGet("Income/Edit/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Edit(int id)
        {
            var income = await dBContext.Incomes.FindAsync(id);
            return View(income);
        }

        [HttpPost("Income/EditIncome")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> EditIncome([FromForm] Income incomeModel, [FromForm] int id)
        {
            // System.InvalidOperation 
            if (ModelState.IsValid)
            {
                var newIncome = await _incomeService.EditIncome(HttpContext, incomeModel, id);

                if (newIncome == true)
                {
                    return RedirectToAction("GetAllIncomes");
                }
            }
            return RedirectToAction("Edit/" + id);
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
            
            if (ModelState.IsValid)
            {
                await _incomeService.DeleteIncome(id);
                return RedirectToAction("GetAllIncomes");
            }
            else
            {
                return RedirectToAction("GetAllIncomes");
            }
            

        }
    }
}
