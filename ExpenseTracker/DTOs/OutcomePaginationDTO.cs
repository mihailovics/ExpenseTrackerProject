using ExpenseTracker.Models;

namespace ExpenseTracker.DTOs
{
    public class ExpensePaginationDTO
    {
        public int? TotalPages { get; set; }
        public int? CurrentPage { get; set; }
        public decimal ExpenseSum { get; set; }
        public int? PageSize { get; set; }
        public decimal Balance { get; set; }
        public List<Expense> Expenses { get; set; }
    }
}
