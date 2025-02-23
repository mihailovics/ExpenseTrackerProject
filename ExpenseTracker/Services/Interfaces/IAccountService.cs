using ExpenseTracker.Models;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IAccountService
    {
        Task<User> GetUserAsync();
        Task<decimal> GetAllowedMinusAsync();
        Task<Account> GetAccountForUserAsync(string userId);

    }
}
