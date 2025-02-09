using System.Diagnostics;
using System.Runtime.CompilerServices;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ExpenseTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICommonMethods _commonMethods;
        private readonly UserManager<User> _userManager;
        private readonly IIncomeService _incomeService;
        private readonly IExpenseService _expenseService;
        public HomeController(ILogger<HomeController> logger, ICommonMethods commonMethods, UserManager<User> userManager,IIncomeService incomeService, IExpenseService expenseService)
        {
            _logger = logger;
            _commonMethods = commonMethods;
            _userManager = userManager;
            _incomeService = incomeService;
            _expenseService = expenseService;
        }

        public async Task<IActionResult> Index()
        {
            var incomeChartData = await _incomeService.GetIncomeChartDataAsync();
            var expenseChartData = await _expenseService.GetExpenseChartDataAsync();

            ViewBag.Labels = incomeChartData.Select(c => c.SourceName).ToArray();
            ViewBag.Data = incomeChartData.Select(c => c.TotalIncome).ToArray();
            ViewBag.ExpenseLabels = expenseChartData.Select(e => e.SourceName).ToArray();
            ViewBag.ExpenseData = expenseChartData.Select(e => e.TotalIncome).ToArray();

            decimal totalIncome = await _incomeService.GetAllIncomeSum(); 
            decimal totalExpense = await _expenseService.GetAllExpenseSum(); 
            decimal balance = totalIncome - totalExpense;
            decimal allowedMinus = 500; 

            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.Balance = balance;
            ViewBag.AllowedMinus = allowedMinus;

            return View();
        }
        [Authorize(Roles = "user")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
