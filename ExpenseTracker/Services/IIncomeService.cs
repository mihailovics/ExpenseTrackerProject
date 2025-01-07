using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Services
{
    public interface IIncomeService
    {
        Task<IncomePaginationDTO> GetPaginatedIncomes(HttpContext httpContext);
        Task<IncomePaginationDTO> GetAllIncomes(HttpContext httpContext);
        Task<Income> NewIncome(HttpContext httpContext, Income incomeModel);
        Task DeleteIncome(int id);

    }
}
