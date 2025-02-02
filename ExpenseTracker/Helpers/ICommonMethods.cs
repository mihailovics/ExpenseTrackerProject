using ExpenseTracker.Models;

namespace ExpenseTracker.Helpers
{
    public interface ICommonMethods
    {
        // Mogu napraviti po jednu koja ce da prima sva tri parametra i da vrati ono sto od ta dva fali 
        Task<List<int>> GetYears(string model, string userId, int? month = null, int? source = null);
        Task<List<int>> GetMonths(string model, string userId, int? year = null, int? source = null);
        Task<List<int>> GetSources(string model, string userId, int? year = null, int? month = null);
        Task<Account> GetAccountForUserAsync(string userId);
        
    }
}
