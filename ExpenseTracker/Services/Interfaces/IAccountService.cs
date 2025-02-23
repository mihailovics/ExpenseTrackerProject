using ExpenseTracker.Models;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IAccountService
    {
        Task<User> GetUserAsync(); 
    }
}
