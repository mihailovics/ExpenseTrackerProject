using ExpenseTracker.Models;

namespace ExpenseTracker.Helpers
{
    public interface ICommonMethods
    {
        Task<List<int>> GetYears(string model, string userId, int? month = null, int? source = null);
        Task<List<int>> GetMonths(string model, string userId, int? year = null, int? source = null);
        Task<List<Source>> GetSources(string model, string userId, int? year = null, int? month = null);
        Task<Account> GetAccountForUserAsync(string userId);
        Task<List<Source>> ShowSources();
    }
}
