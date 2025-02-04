using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Services
{
    public interface IIncomeService
    {
        Task<PaginationViewModel> GetPaginatedIncomes(int? year, int? month, int? source, int? pageNumber, int? pageSize);
        Task<bool> NewIncome(string userId, IncomeViewModel incomeModel);
        Task<bool> EditIncome(string userId, Income updatedIncome, int id);
        Task DeleteIncome(int id);
        Task<Income> FindByid(int id);

    }
}
