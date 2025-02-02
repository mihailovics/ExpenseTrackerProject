using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class ExpenseService : IExpenseService
    {
        ApplicationDBContext dBContext;
        private readonly UserManager<User> _userManager;
        private readonly ICommonMethods _commonMethods;

        public ExpenseService(ApplicationDBContext DbContext, UserManager<User> userManager, ICommonMethods commonMethods)
        {
            dBContext = DbContext;
            _userManager = userManager;
            _commonMethods = commonMethods;
        }

        public async Task DeleteExpense(int id)
        {
            var Expense = await dBContext.Expenses.FindAsync(id);

            if (Expense != null) 
            {
                dBContext.Expenses.Remove(Expense);
                await dBContext.SaveChangesAsync();
            }
        }

        public async Task<ExpensePaginationDTO> GetPaginatedExpenses(HttpContext httpContext)
        {
            // Pitanje da li treba sve da promenim da ne koristim uopste httpContext
            var pageNumberString = httpContext.Request.Query["pageNumber"].FirstOrDefault();
            int pageNumber = string.IsNullOrEmpty(pageNumberString) ? 1 : int.Parse(pageNumberString);

            var pageSizeString = httpContext.Request.Query["pageSize"].FirstOrDefault();
            int pageSize = string.IsNullOrEmpty(pageSizeString) ? 5 : int.Parse(pageSizeString);

            var years = httpContext.Request.Query["year"].FirstOrDefault();
            var month = httpContext.Request.Query["month"].FirstOrDefault();
            var source = httpContext.Request.Query["source"].FirstOrDefault();

            var user = await _userManager.GetUserAsync(httpContext.User);
            var account = await _commonMethods.GetAccountForUserAsync(user.Id);

            IQueryable<Expense> query = dBContext.Expenses.Where(i => i.AccountId == account.Id);

            if (!string.IsNullOrEmpty(years))
            {
                var yearInt = int.Parse(years);
                query = query.Where(i => i.CreatedAt.Year == yearInt);
            }

            if (!string.IsNullOrEmpty(month))
            {
                var monthInt = int.Parse(month);
                query = query.Where(i => i.CreatedAt.Month == monthInt);
            }

            if (!string.IsNullOrEmpty(source))
            {
                var sourceInt = int.Parse(source);
                query = query.Where(i => i.SourceId == sourceInt);
            }

            int totalExpenses = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalExpenses / (double)pageSize);
            List<Expense> pagedExpenses = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            decimal ExpenseSum = pagedExpenses.Sum(i => i.ExpenseAmount);

            return new ExpensePaginationDTO
            {
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                Expenses = pagedExpenses,
                PageSize = pageSize,
                Balance = account.Balance,
                ExpenseSum = ExpenseSum
            };
        }

        public async Task<bool> NewExpense(string userId, Expense ExpenseModel)
        {
            try
            {
                var account = await _commonMethods.GetAccountForUserAsync(userId);

                ExpenseModel.Account = account;
                ExpenseModel.AccountId = account.Id;
                ExpenseModel.CreatedAt = DateTime.Now;

                dBContext.Expenses.Add(ExpenseModel);
                await dBContext.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException) 
            {
                return false;
            }
        }

        public async Task<bool> EditExpense(string userId, Expense ExpenseModel, int id)
        {
            try
            {
                var account = await _commonMethods.GetAccountForUserAsync(userId);

                var Expense = await dBContext.Expenses.FirstOrDefaultAsync(i => i.Id == id && i.AccountId == account.Id);

                if (userId == null)
                {
                    return false;
                }

                Expense.ExpenseAmount = ExpenseModel.ExpenseAmount;
                Expense.Description = ExpenseModel.Description;
                Expense.Source = ExpenseModel.Source;
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
    }
}
