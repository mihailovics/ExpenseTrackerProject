namespace ExpenseTracker.ViewModels
{
    public class HomeViewModel
    {
        public HomeViewModel() { }

        public HomeViewModel(List<string> expenseLabels, List<decimal> expenseData, List<string> incomeLabels, 
            List<decimal> incomeData, decimal totalIncome, decimal totalExpense, decimal balance, decimal allowedMinus)
        {
            ExpenseLabels = expenseLabels;
            ExpenseData = expenseData;
            IncomeLabels = incomeLabels;
            IncomeData = incomeData;
            TotalIncome = totalIncome;
            TotalExpense = totalExpense;
            Balance = balance;
            AllowedMinus = allowedMinus;
        }

        public List<string> ExpenseLabels { get; set; }
        public List<decimal> ExpenseData { get; set; }
        public List<string> IncomeLabels { get; set; }
        public List<decimal> IncomeData { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public decimal AllowedMinus { get; set; }

    }
}
