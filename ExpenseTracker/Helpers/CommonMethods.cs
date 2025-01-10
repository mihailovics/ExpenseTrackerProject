
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ExpenseTracker.Helpers
{
    public class CommonMethods : ICommonMethods
    {
        ApplicationDBContext dBContext;
        private readonly UserManager<User> _userManager;

        public CommonMethods(ApplicationDBContext DBContext, UserManager<User> userManager)
        {
           dBContext = DBContext;
           _userManager = userManager; 
        }

        public async Task<List<int>> GetDistinctMonthsAsync(HttpContext httpContext)
        {
            var userId =  _userManager.GetUserId(httpContext.User);    

            return await dBContext.Incomes
            .Where(i => i.UserId == userId)
            .Select(i => i.CreatedAt.Month)
            .Distinct()
            .ToListAsync();
        }

        public async Task<List<string>> GetDistinctSourcesAsync(HttpContext httpContext)
        {
            var userId = _userManager.GetUserId(httpContext.User);

            return await dBContext.Incomes
            .Where(i => i.UserId == userId)
            .Select(i => i.Source)
            .Distinct()
            .ToListAsync();
        }

        public async Task<List<int>> GetDistinctYearsAsync(HttpContext httpContext)
        {
            var userId = _userManager.GetUserId(httpContext.User);

            return await dBContext.Incomes
            .Where(i => i.UserId == userId)
            .Select(i => i.CreatedAt.Year)
            .Distinct()
            .ToListAsync();
        }

        
    }
}
