using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.DTOs
{
    public class PaginationViewModel
    {
        public int? TotalPages { get; set; }
        public int? CurrentPage { get; set; }
        public decimal IncomeSum { get; set; }
        public int? PageSize { get; set; }
        public decimal Balance { get; set; }

        public int? SelectedYear { get; set; }
        public int? SelectedMonth { get; set; }
        public int? SelectedSource { get; set; }

        public List<SelectListItem> Sources { get; set; }
        public List<SelectListItem> Years { get; set; }
        public List<SelectListItem> Months { get; set; }

        public List<Income>? Incomes { get; set; }
        public List<Expense>? Expenses { get; set; }
    }
}
