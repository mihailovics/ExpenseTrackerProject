using Elfie.Serialization;
using System.Drawing.Printing;
using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using ExpenseTracker.Services.Interfaces;

namespace ExpenseTracker.Services
{
    public class ExpenseService : IExpenseService
    {
        ApplicationDBContext dBContext;
        private readonly UserManager<User> _userManager;
        private readonly ICommonMethods _commonMethods;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExpenseService(ApplicationDBContext DbContext, UserManager<User> userManager, ICommonMethods commonMethods, IHttpContextAccessor httpContextAccessor)
        {
            dBContext = DbContext;
            _userManager = userManager;
            _commonMethods = commonMethods;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> DeleteExpense(int id)
        {
            var Expense = await dBContext.Expenses.FindAsync(id);

            if (Expense != null) 
            {
                dBContext.Expenses.Remove(Expense);
                await dBContext.SaveChangesAsync();
                return true;
            }
            else { return false; }  
        }

        public async Task<PaginationViewModel> GetPaginatedExpenses(int? year, int? month, int? source, int? PageNumber, int? PageSize)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
            var account = await _commonMethods.GetAccountForUserAsync(user.Id);

            IQueryable<Expense> query = dBContext.Expenses.Where(i => i.AccountId == account.Id);

            if (year.HasValue)
            {
                query = query.Where(e => e.CreatedAt.Year == year);
            }

            if (month.HasValue)
            {
                query = query.Where(e => e.CreatedAt.Month == month);
            }

            if (source.HasValue)
            {
                query = query.Where(e => e.SourceId == source);
            }

            int pageNumber = PageNumber == null ? 1 : (int)PageNumber;
            int pageSize = PageSize == null ? 5 : (int)PageSize;
            int totalIncomes = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalIncomes / (double)pageSize);
            List<Expense> pagedExpenses = await query
                .Skip((int)((pageNumber - 1) * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            decimal expenseSum = pagedExpenses.Sum(e => e.ExpenseAmount);

            var sources = await _commonMethods.GetSources("expense", user.Id, year, month);
            var years = await _commonMethods.GetYears("expense", user.Id, month, source);
            var months = await _commonMethods.GetMonths("expense", user.Id, year, source);


            return new PaginationViewModel
            {
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                Expenses = pagedExpenses,
                PageSize = pageSize,
                Balance = account.Balance,
                Sum = expenseSum,
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
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList(),
            };
        }

        public async Task<bool> NewExpense(string userId, GeneralViewModel expenseModel)
        {
            try
            {
                var account = await _commonMethods.GetAccountForUserAsync(userId);

                var expense = new Expense
                {
                    ExpenseAmount = expenseModel.Amount,
                    Account = account,
                    AccountId = account.Id,
                    CreatedAt = DateTime.Now,
                    Description = expenseModel.Description,
                    SourceId = expenseModel.SourceId,
                };

                dBContext.Expenses.Add(expense);
                await dBContext.SaveChangesAsync();

                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> EditExpense(string userId, GeneralViewModel ExpenseModel, int id)
        {
            try
            {
                var account = await _commonMethods.GetAccountForUserAsync(userId);

                var Expense = await dBContext.Expenses.FirstOrDefaultAsync(i => i.Id == id && i.AccountId == account.Id);

                if (userId == null)
                {
                    return false;
                }

                Expense.ExpenseAmount = ExpenseModel.Amount;
                Expense.Description = ExpenseModel.Description;
                Expense.SourceId = ExpenseModel.SourceId;
                Expense.AccountId = account.Id;

                await dBContext.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                return false;
            }
        }

        public async Task<Expense> FindByid(int id)
        { 
            var expense = await dBContext.Expenses.FindAsync(id);
            return expense;
        }
        public async Task<List<ChartViewModel>> GetExpenseChartDataAsync()
        {
            var expenses = await dBContext.Expenses
            .GroupBy(i => i.SourceId)
            .Select(g => new ChartViewModel
            {
                SourceName = dBContext.Sources
                               .Where(s => s.Id == g.Key)
                               .Select(s => s.Name)
                               .FirstOrDefault(),
                TotalIncome = g.Sum(i => i.ExpenseAmount)
            })
            .Where(g => g.SourceName != null)
            .ToListAsync();

            return expenses;
        }

        public async Task<decimal> GetAllExpenseSum()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
            var account = await _commonMethods.GetAccountForUserAsync(user.Id);

            return dBContext.Expenses
                .Where(i => i.AccountId == account.Id)
                .Sum(i => i.ExpenseAmount);
        }
        // A mogao sam napraviti i u common methods univerzalnu, ako ima potrebe napravicemo
        public async Task<GeneralViewModel> NewExpenseView()
        {
            var viewModel = new GeneralViewModel
            {
                Sources = (await _commonMethods.ShowSources())
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList()
            };

            return viewModel;
        }
    }
}
