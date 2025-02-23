using ExpenseTracker.Services.Interfaces;

namespace ExpenseTracker.Services
{
    public class HomeService : IHomeService
    {
        private readonly IIncomeService _incomeService;
        private readonly IExpenseService _expenseService;


    }
}
