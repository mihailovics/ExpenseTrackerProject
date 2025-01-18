
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
            var account = await GetAccountForUserAsync(userId);

            return await dBContext.Incomes
            .Where(i => i.AccountId == account.Id)
            .Select(i => i.CreatedAt.Month)
            .Distinct()
            .ToListAsync();
        }

        public async Task<List<string>> GetDistinctSourcesAsync(HttpContext httpContext)
        {
            var userId = _userManager.GetUserId(httpContext.User);
            var account = await GetAccountForUserAsync(userId);
            return await dBContext.Incomes
            .Where(i => i.AccountId == account.Id)
            .Select(i => i.Source)
            .Distinct()
            .ToListAsync();
        }

        public async Task<List<int>> GetDistinctYearsAsync(HttpContext httpContext)
        {
            var userId = _userManager.GetUserId(httpContext.User);
            var account = await GetAccountForUserAsync(userId);
            return await dBContext.Incomes
            .Where(i => i.AccountId == account.Id)
            .Select(i => i.CreatedAt.Year)
            .Distinct()
            .ToListAsync();
        }
        public async Task<Account> GetAccountForUserAsync(string userId)
        {
            
            var account = await dBContext.Accounts
                .Include(a => a.User) 
                .FirstOrDefaultAsync(a => a.UserId == userId);

            return account;
        }

    }
}
