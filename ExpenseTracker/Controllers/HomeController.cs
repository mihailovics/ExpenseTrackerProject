using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace ExpenseTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IHomeService _homeService;
        public HomeController(IAccountService accountService, IHomeService homeService)
        {
            _accountService = accountService;
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _accountService.GetUserAsync();

            if (user != null)
            {  
                var homeModel = await _homeService.GetHomeViewAsync();

                return View(homeModel);
            }
            else
            {
                return Redirect("/Identity/Account/Login");
            }
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
