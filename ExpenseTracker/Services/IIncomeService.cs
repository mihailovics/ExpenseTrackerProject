using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Services
{
    public interface IIncomeService
    {
        Task<IncomePaginationDTO> GetPaginatedIncomes(HttpContext httpContext);
        Task<bool> NewIncome(string userId, Income incomeModel);
        Task<bool> EditIncome(string userId, Income updatedIncome, int id);
        Task DeleteIncome(int id);
        Task<Income> FindByid(int id);

    }
}
