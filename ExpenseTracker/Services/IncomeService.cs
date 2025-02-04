using System.Linq;
using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace ExpenseTracker.Services
{
    public class IncomeService : IIncomeService
    {
        ApplicationDBContext dBContext;
        private readonly UserManager<User> _userManager;
        private readonly ICommonMethods _commonMethods;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IncomeService(ApplicationDBContext DbContext, UserManager<User> userManager, ICommonMethods commonMethods, IHttpContextAccessor httpContextAccessor) 
        {
            dBContext = DbContext;
            _userManager = userManager;
            _commonMethods = commonMethods;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginationViewModel> GetPaginatedIncomes(int? year, int? month, int? source, int? PageNumber, int? PageSize)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
            var account = await _commonMethods.GetAccountForUserAsync(user.Id);

            IQueryable<Income> query = dBContext.Incomes.Where(i => i.AccountId == account.Id);

            if (year.HasValue)
            {
                query = query.Where(i => i.CreatedAt.Year == year);
            }

            if (month.HasValue)
            {
                query = query.Where(i => i.CreatedAt.Month == month);
            }

            if (source.HasValue)
            {
                query = query.Where(i => i.SourceId == source);
            }

            int pageNumber = PageNumber==null ? 1 : (int)PageNumber;
            int pageSize = PageSize==null ? 5 : (int)PageSize;
            int totalIncomes = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)(totalIncomes / pageSize));
            List<Income> pagedIncomes = await query
                .Skip((int)((pageNumber - 1) * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            decimal incomeSum = pagedIncomes.Sum(i => i.IncomeAmount);

            var sources = await _commonMethods.GetSources("income", user.Id, year, month);
            var years = await _commonMethods.GetYears("income", user.Id, month, source);
            var months = await _commonMethods.GetMonths("income", user.Id, year, source);


            return new PaginationViewModel
            {
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                Incomes = pagedIncomes,
                PageSize = pageSize,
                Balance = account.Balance,
                IncomeSum = incomeSum,
                SelectedMonth = month,
                SelectedSource = source,
                SelectedYear = year,
                Years = years.Select(y => new SelectListItem
                {
                    Value = y.ToString(),
                    Text = y.ToString()
                }).ToList(),
                Months = months.Select(m => new SelectListItem
                {
                    Value = m.ToString(),
                    Text = m.ToString()
                }).ToList(),
                Sources = sources.Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString()
                }).ToList(),
            };
        }

        public async Task<bool> NewIncome(string userId, IncomeViewModel incomeModel)
        {
            try
            {
                var account = await _commonMethods.GetAccountForUserAsync(userId);

                var income = new Income
                {
                    IncomeAmount = incomeModel.IncomeAmount,
                    Account = account,
                    AccountId = account.Id,
                    CreatedAt = DateTime.Now,
                    Description = incomeModel.Description,
                    SourceId = incomeModel.SourceId,
                };
                
                dBContext.Incomes.Add(income);
                await dBContext.SaveChangesAsync();

                return true;
            }
            catch(SqlException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public async Task DeleteIncome(int id)
        {
            var income = await dBContext.Incomes.FindAsync(id);
            if (income != null) 
            {
                dBContext.Incomes.Remove(income);
                await dBContext.SaveChangesAsync();
            }
        }

        public async Task<bool> EditIncome(string userId, Income updatedIncome, int id)
        {
            var account = await _commonMethods.GetAccountForUserAsync(userId);

            var income = await dBContext.Incomes.FirstOrDefaultAsync(i => i.Id == id && i.AccountId == account.Id);

            if(userId == null)
            {
                return false;
            }

            income.IncomeAmount = updatedIncome.IncomeAmount;
            income.Description = updatedIncome.Description;
            income.Source = updatedIncome.Source;
            income.AccountId = account.Id;

            await dBContext.SaveChangesAsync();

            return true;
        }

        public async Task<Income> FindByid(int id)
        {
            var income = await dBContext.Incomes.FindAsync(id);
            return income;  
        }
    }
}
