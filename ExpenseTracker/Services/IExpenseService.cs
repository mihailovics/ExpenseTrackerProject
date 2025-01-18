using ExpenseTracker.DTOs;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services
{
    public interface IExpenseService
    {
        Task<ExpensePaginationDTO> GetPaginatedExpenses(HttpContext httpContext);
        Task<ExpensePaginationDTO> GetAllExpenses(HttpContext httpContext);
        Task<Expense> NewExpense(HttpContext httpContext, Expense ExpenseModel);
        Task<bool> EditExpense(HttpContext httpContext, Expense ExpenseModel, int id);
        Task DeleteExpense(int id);
    }
}
