using ExpenseTracker.Models;

namespace ExpenseTracker.DTOs
{
    public class OutcomePaginationDTO
    {
        public int? TotalPages { get; set; }
        public int? CurrentPage { get; set; }
        public decimal OutcomeSum { get; set; }
        public int? PageSize { get; set; }
        public decimal Balance { get; set; }
        public List<Outcome> Outcomes { get; set; }
    }
}
