using ExpenseTracker.Services.Interfaces;
using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Services
{
    public class HomeService : IHomeService
    {
        private readonly IIncomeService _incomeService;
        private readonly IExpenseService _expenseService;
        private readonly IAccountService _accountService;
        public HomeService(IIncomeService incomeService, IExpenseService expenseService, IAccountService accountService)
        {
            _incomeService = incomeService;
            _expenseService = expenseService;
            _accountService = accountService;
        }
        
        public async Task<HomeViewModel> GetHomeViewAsync()
        {
            var incomeChartData = await _incomeService.GetIncomeChartDataAsync();
            var expenseChartData = await _expenseService.GetExpenseChartDataAsync();

            var totalIncome = await _incomeService.GetAllIncomeSum();
            var totalExpense = await _expenseService.GetAllExpenseSum();

            return new HomeViewModel
            {
                IncomeLabels = incomeChartData.Select(c => c.SourceName).ToList(),
                IncomeData = incomeChartData.Select(c => c.TotalAmount).ToList(),
                ExpenseLabels = expenseChartData.Select(c => c.SourceName).ToList(),
                ExpenseData = expenseChartData.Select(c => c.TotalAmount).ToList(),
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense,
                AllowedMinus = await _accountService.GetAllowedMinusAsync()
            };
        }
    }
}
