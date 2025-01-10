using ExpenseTracker.DTOs;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services
{
    public interface IOutcomeService
    {
        Task<OutcomePaginationDTO> GetPaginatedOutcomes(HttpContext httpContext);
        Task<OutcomePaginationDTO> GetAllOutcomes(HttpContext httpContext);
        Task<Outcome> NewOutcome(HttpContext httpContext, Outcome outcomeModel);
        Task<bool> EditOutcome(HttpContext httpContext, Outcome outcomeModel, int id);
        Task DeleteOutcome(int id);
    }
}
