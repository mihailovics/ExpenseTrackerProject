using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Services
{
    public interface IIncomeService
    {
        Task<PaginationViewModel> GetPaginatedIncomes(int? year, int? month, int? source, int? pageNumber, int? pageSize);
        Task<bool> NewIncome(string userId, ViewModel incomeModel);
        Task<bool> EditIncome(string userId, ViewModel updatedIncome, int id);
        Task<bool> DeleteIncome(int id);
        Task<Income> FindByid(int id);
        Task<List<ChartViewModel>> GetIncomeChartDataAsync();
        Task<decimal> GetAllIncomeSum();

    }
}
