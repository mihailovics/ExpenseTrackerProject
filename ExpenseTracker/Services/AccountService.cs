using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class AccountService : IAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDBContext _dbContext;

        public AccountService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, ApplicationDBContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<Account> GetAccountForUserAsync(string userId)
        {
            var account = await _dbContext.Account
                .FirstOrDefaultAsync(a => a.UserId == userId);

            return account;
        }

        public async Task<decimal> GetAllowedMinusAsync()
        {
            var user = await GetUserAsync();
            var account = await GetAccountForUserAsync(user.Id);

            return account.AllowedMinus;
        }

        public async Task<decimal> GetBalanceAsync()
        {
            var user = await GetUserAsync();
            var account = await GetAccountForUserAsync(user.Id);

            return account.Balance;
        }

        public async Task<User> GetUserAsync()
        {
            try
            {
                return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
            }
            catch (Exception ex) 
            {
                return null;
            }
        }
    }
}
