using ExpenseTracker.DTOs;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services
{
    public interface IExpenseService
    {
        Task<PaginationViewModel> GetPaginatedExpenses(int? year, int? month, int? source, int? PageNumber, int? PageSize);
        Task<bool> NewExpense(string userId, GeneralViewModel expenseModel);
        Task<bool> EditExpense(string userId, GeneralViewModel ExpenseModel, int id);
        Task<bool> DeleteExpense(int id);
        Task<Expense> FindByid(int id);
        Task<List<ChartViewModel>> GetExpenseChartDataAsync();
        Task<decimal> GetAllExpenseSum();
        Task<GeneralViewModel> NewExpenseView();
    }
}
