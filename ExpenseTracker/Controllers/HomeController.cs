using System.Diagnostics;
using System.Runtime.CompilerServices;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
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
        public HomeController(ILogger<HomeController> logger, ICommonMethods commonMethods, UserManager<User> userManager)
        {
            _logger = logger;
            _commonMethods = commonMethods;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            try 
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var account = await _commonMethods.GetAccountForUserAsync(user.Id);

                    ViewBag.Balance = account.Balance;
                }
            }
            catch (Exception ex) 
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
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
