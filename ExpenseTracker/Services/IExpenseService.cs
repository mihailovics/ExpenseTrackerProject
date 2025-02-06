using ExpenseTracker.DTOs;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services
{
    public interface IExpenseService
    {
        Task<PaginationViewModel> GetPaginatedExpenses(int? year, int? month, int? source, int? PageNumber, int? PageSize);
        Task<bool> NewExpense(string userId, IncomeViewModel expenseModel);
        Task<bool> EditExpense(string userId, Expense ExpenseModel, int id);
        Task DeleteExpense(int id);
        Task<Expense> FindByid(int id);
    }
}
