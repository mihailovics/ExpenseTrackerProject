using ExpenseTracker.Models;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Services
{
    public class AccountService : IAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public AccountService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
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
