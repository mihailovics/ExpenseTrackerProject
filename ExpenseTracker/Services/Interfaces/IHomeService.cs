using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IHomeService
    {
        Task<HomeViewModel> GetHomeViewAsync();
    }
}
