using ExpenseTracker.Models;

namespace ExpenseTracker.Helpers
{
    public interface ICommonMethods
    {
        // Mogu napraviti po jednu koja ce da prima sva tri parametra i da vrati ono sto od ta dva fali 
        Task<List<int>> GetYearsIncome(HttpContext httpContext, int? month = null, string? source = null);
        Task<List<int>> GetMonthsIncome(HttpContext httpContext, int? year = null, string? source = null);
        Task<List<string>> GetSourcesIncome(HttpContext httpContext, int? year = null, int? month = null);
        Task<List<int>> GetYearsExpense(HttpContext httpContext, int? month = null, string? source = null);
        Task<List<int>> GetMonthsExpense(HttpContext httpContext, int? year = null, string? source = null);
        Task<List<string>> GetSourcesExpense(HttpContext httpContext, int? year = null, int? month = null);
        Task<List<object>> GetFiltersIncome(HttpContext httpContext, int? year = null, int? month = null, string? source = null);
        Task<List<object>> GetFiltersExpenses(HttpContext httpContext, int? year = null, int? month = null, string? source = null);
        Task<Account> GetAccountForUserAsync(string userId);
        
    }
}
