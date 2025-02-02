
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
        //Skratiti kod, ponavlja se smisliti nacin kako
        public async Task<List<int>> GetMonths(string model, string userId, int? year = null, int? source = null)
        { 
            var account = await GetAccountForUserAsync(userId);

            if (model == "income")
            {
                var query = dBContext.Incomes
                   .Where(i => i.AccountId == account.Id);

                if (year.HasValue)
                {
                    query = query.Where(i => i.CreatedAt.Year == year.Value);
                }

                if (source.HasValue)
                {
                    query = query.Where(i => i.SourceId == source);
                }

                return await query
                    .Select(i => i.CreatedAt.Month)
                    .Distinct()
                    .ToListAsync();
            }
            else
            {
                var query = dBContext.Expenses.Where(i => i.AccountId == account.Id);

                if (year.HasValue)
                {
                    query = query.Where(i => i.CreatedAt.Year == year.Value);
                }

                if (source.HasValue)
                {
                    query = query.Where(i => i.SourceId == source);
                }

                return await query
                    .Select(i => i.CreatedAt.Month)
                    .Distinct()
                    .ToListAsync();
            }
        }

        public async Task<List<int>> GetSources(string model, string userId, int? year = null, int? month = null)
        {
            var account = await GetAccountForUserAsync(userId);

            if (model == "income")
            {
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
                    .Select(i => i.SourceId)
                    .Distinct()
                    .ToListAsync();
            }
            else
            {
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
                    .Select(i => i.SourceId)
                    .Distinct()
                    .ToListAsync();
            }
            
        }

        public async Task<List<int>> GetYears(string model, string userId, int? month = null, int? source = null)
        {
            var account = await GetAccountForUserAsync(userId);

            if (model == "income")
            {
                var query = dBContext.Incomes
                    .Where(i => i.AccountId == account.Id);

                if (month.HasValue)
                {
                    query = query.Where(i => i.CreatedAt.Month == month.Value);
                }

                if (source.HasValue)
                {
                    query = query.Where(i => i.SourceId == source);
                }

                return await query
                    .Select(i => i.CreatedAt.Year)
                    .Distinct()
                    .ToListAsync();
            }
            else
            {
                var query = dBContext.Expenses
                   .Where(i => i.AccountId == account.Id);

                if (month.HasValue)
                {
                    query = query.Where(i => i.CreatedAt.Month == month.Value);
                }

                if (source.HasValue)
                {
                    query = query.Where(i => i.SourceId == source);
                }

                return await query
                    .Select(i => i.CreatedAt.Year)
                    .Distinct()
                    .ToListAsync();
            }
        }

        public async Task<Account> GetAccountForUserAsync(string userId)
        {
            var account = await dBContext.Account
                .FirstOrDefaultAsync(a => a.UserId == userId);

            return account;
        }
    }
}
