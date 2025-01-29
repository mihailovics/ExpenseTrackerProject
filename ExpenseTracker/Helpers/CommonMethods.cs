
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

        public async Task<List<int>> GetMonthsIncome(HttpContext httpContext, int? year = null, string? source = null)
        { 
            var userId = _userManager.GetUserId(httpContext.User);
            
            var account = await GetAccountForUserAsync(userId);
            
            var query = dBContext.Incomes
                .Where(i => i.AccountId == account.Id);

            if (year.HasValue)
            {
                query = query.Where(i => i.CreatedAt.Year == year.Value);
            }

            if (!string.IsNullOrEmpty(source))
            {
                query = query.Where(i => i.Source == source);
            }

            return await query
                .Select(i => i.CreatedAt.Month)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetSourcesIncome(HttpContext httpContext, int? year = null, int? month = null)
        {
            var userId = _userManager.GetUserId(httpContext.User);

            var account = await GetAccountForUserAsync(userId);

            var query = dBContext.Incomes
                .Where(i => i.AccountId == account.Id);

            if (year.HasValue)
            {
                query = query.Where(i => i.CreatedAt.Year == year.Value);
            }

            if (month.HasValue)
            {
                query = query.Where(i => i.CreatedAt.Month == month.Value);
            }

            return await query
                .Select(i => i.Source)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<int>> GetYearsIncome(HttpContext httpContext, int? month = null, string? source = null)
        {
            var userId = _userManager.GetUserId(httpContext.User);

            var account = await GetAccountForUserAsync(userId);

            var query = dBContext.Incomes
                .Where(i => i.AccountId == account.Id);

            if (month.HasValue)
            {
                query = query.Where(i => i.CreatedAt.Month == month.Value);
            }

            if (!string.IsNullOrEmpty(source))
            {
                query = query.Where(i => i.Source == source);
            }

            return await query
                .Select(i => i.CreatedAt.Year)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<int>> GetYearsExpense(HttpContext httpContext, int? month = null, string? source = null)
        {
            var userId = _userManager.GetUserId(httpContext.User);

            var account = await GetAccountForUserAsync(userId);

            var query = dBContext.Expenses
                .Where(i => i.AccountId == account.Id);

            if (month.HasValue)
            {
                query = query.Where(i => i.CreatedAt.Month == month.Value);
            }

            if (!string.IsNullOrEmpty(source))
            {
                query = query.Where(i => i.Source == source);
            }

            return await query
                .Select(i => i.CreatedAt.Year)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<int>> GetMonthsExpense(HttpContext httpContext, int? year = null, string? source = null)
        {
            var userId = _userManager.GetUserId(httpContext.User);

            var account = await GetAccountForUserAsync(userId);

            var query = dBContext.Expenses
                .Where(i => i.AccountId == account.Id);

            if (year.HasValue)
            {
                query = query.Where(i => i.CreatedAt.Year == year.Value);
            }

            if (!string.IsNullOrEmpty(source))
            {
                query = query.Where(i => i.Source == source);
            }

            return await query
                .Select(i => i.CreatedAt.Month)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetSourcesExpense(HttpContext httpContext, int? year = null, int? month = null)
        {
            var userId = _userManager.GetUserId(httpContext.User);

            var account = await GetAccountForUserAsync(userId);

            var query = dBContext.Expenses
                .Where(i => i.AccountId == account.Id);

            if (year.HasValue)
            {
                query = query.Where(i => i.CreatedAt.Year == year.Value);
            }

            if (month.HasValue)
            {
                query = query.Where(i => i.CreatedAt.Month == month.Value);
            }

            return await query
                .Select(i => i.Source)
                .Distinct()
                .ToListAsync();
        }

        public async Task<Account> GetAccountForUserAsync(string userId)
        {

            var account = await dBContext.Account
                .FirstOrDefaultAsync(a => a.UserId == userId);

            return account;
        }

        public async Task<List<object>> GetFiltersIncome(HttpContext httpContext, int? year = null, int? month = null, string? source = null)
        {
            var userId = _userManager.GetUserId(httpContext.User);

            var account = await GetAccountForUserAsync(userId);

            var query = dBContext.Incomes
                .Where(i => i.AccountId == account.Id);

            return null;

        }

        public Task<List<object>> GetFiltersExpenses(HttpContext httpContext, int? year = null, int? month = null, string? source = null)
        {
            throw new NotImplementedException();
        }
    }
}
