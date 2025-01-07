using ExpenseTracker.Models;

namespace ExpenseTracker.DTOs
{
    public class IncomePaginationDTO
    {
        public int? TotalPages { get; set; }
        public int? CurrentPage { get; set; }
        public decimal IncomeSum { get; set; }
        public int? PageSize { get; set; }
        public decimal Balance { get; set; }
        public List<Income> Incomes { get; set; }
    }
}
