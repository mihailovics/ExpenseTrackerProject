namespace ExpenseTracker.ViewModels
{
    public class HomeViewModel
    {
        public List<string> ExpenseLabels { get; set; }
        public List<double> ExpenseData { get; set; }
        public List<string> IncomeLabels { get; set; }
        public List<double> IncomeData { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public decimal AllowedMinus {  get; set; }

    }
}
