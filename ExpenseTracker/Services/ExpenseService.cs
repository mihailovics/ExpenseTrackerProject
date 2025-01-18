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

        public async Task<ExpensePaginationDTO> GetAllExpenses(HttpContext httpContext)
        {
            List<Expense> AllExpenses = new List<Expense>();

           
            var user = await _userManager.GetUserAsync(httpContext.User);
            var account = await _commonMethods.GetAccountForUserAsync(user.Id);

            AllExpenses = await dBContext.Expenses.Where(i => i.AccountId == account.Id).ToListAsync();

            decimal ExpenseSum = AllExpenses.Sum(i => i.ExpenseAmount);

            return new ExpensePaginationDTO
            {
                ExpenseSum = ExpenseSum,
                Balance = account.Balance,
                Expenses = AllExpenses
            };
        }

        public async Task<ExpensePaginationDTO> GetPaginatedExpenses(HttpContext httpContext)
        {
            List<Expense> AllExpenses = new List<Expense>();
            var pageNumberString = httpContext.Request.Query["pageNumber"].FirstOrDefault();
            int pageNumber = string.IsNullOrEmpty(pageNumberString) ? 1 : int.Parse(pageNumberString);
            int pageSize = 5;

            
            var user = await _userManager.GetUserAsync(httpContext.User);
            var account = await _commonMethods.GetAccountForUserAsync(user.Id);

            IQueryable<Expense> query = dBContext.Expenses.Where(i => i.AccountId == account.Id);

            int totalIncomes = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalIncomes / (double)pageSize);
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

        public async Task<Expense> NewExpense(HttpContext httpContext, Expense ExpenseModel)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);
            var account = await _commonMethods.GetAccountForUserAsync(user.Id);

            
            ExpenseModel.Account = account;
            ExpenseModel.AccountId = account.Id;
            ExpenseModel.CreatedAt = DateTime.Now;

            dBContext.Expenses.Add(ExpenseModel);
            await dBContext.SaveChangesAsync();

            return ExpenseModel;
        }

        public async Task<bool> EditExpense(HttpContext httpContext, Expense ExpenseModel, int id)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);
            var account = await _commonMethods.GetAccountForUserAsync(user.Id);

            var Expense = await dBContext.Expenses.FirstOrDefaultAsync(i => i.Id == id && i.AccountId == account.Id);

            if (user == null)
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
    }
}
