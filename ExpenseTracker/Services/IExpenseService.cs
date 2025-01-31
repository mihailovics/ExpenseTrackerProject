using ExpenseTracker.DTOs;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services
{
    public interface IExpenseService
    {
        Task<ExpensePaginationDTO> GetPaginatedExpenses(HttpContext httpContext);
        Task<bool> NewExpense(string userId, Expense ExpenseModel);
        Task<bool> EditExpense(string userId, Expense ExpenseModel, int id);
        Task DeleteExpense(int id);
        Task<Expense> FindByid(int id);
    }
}
