
using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        public async Task<List<Source>> GetSources(string model, string userId, int? year = null, int? month = null)
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

                return await dBContext.Sources
                    .Where(s => dBContext.Incomes
                    .Select(i => i.SourceId)
                    .Distinct()
                    .Contains(s.Id))
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

                return await dBContext.Sources
                     .Where(s => dBContext.Expenses
                     .Select(i => i.SourceId)
                     .Distinct()
                     .Contains(s.Id))
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

        public async Task<List<Source>> ShowSources()
        {
            return await dBContext.Sources.ToListAsync();
        }

      
        public async Task<GeneralViewModel> ConvertIncomeToGeneral(Income IncomeModel)
        {
            var getSources = (await ShowSources())
                  .Select(s => new SelectListItem
                  {
                      Value = s.Id.ToString(),
                      Text = s.Name
                  }).ToList();

            return new GeneralViewModel(IncomeModel.IncomeAmount, IncomeModel.Description, IncomeModel.SourceId, getSources,
                  IncomeModel.CreatedAt, IncomeModel.AccountId, IncomeModel.Account);
        }

        public async Task<GeneralViewModel> ConvertExpenseToGeneral(Expense ExpenseModel)
        {
            var getSources = (await ShowSources())
                  .Select(s => new SelectListItem
                  {
                      Value = s.Id.ToString(),
                      Text = s.Name
                  }).ToList();

            return new GeneralViewModel(ExpenseModel.ExpenseAmount, ExpenseModel.Description, ExpenseModel.SourceId, getSources,
                    ExpenseModel.CreatedAt, ExpenseModel.AccountId, ExpenseModel.Account);
        }
    }
}
