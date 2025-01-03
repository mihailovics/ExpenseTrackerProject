﻿using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    public class OutcomeController : Controller
    {
        ApplicationDBContext dBContext;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;

        public OutcomeController(ILogger<HomeController> logger, UserManager<User> UserManager, ApplicationDBContext DbContext)
        {
            _userManager = UserManager;
            _logger = logger;
            dBContext = DbContext;
        }

        [HttpGet("Outcome/GetAllOutcomes")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetAllOutcomes()
        {
            List<Outcome> AllOutcomes = new List<Outcome>();

            var user = _userManager.GetUserId(User);
            AllOutcomes = await dBContext.Outcomes.Where(i => i.UserId == user).ToListAsync();

            return View(AllOutcomes);
        }
    }
}
