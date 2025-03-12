using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1,999999999, ErrorMessage = "Amount must be greater than 1")]
        public decimal ExpenseAmount { get; set; }
        public string? Description { get; set; }
        [Required]
        public int SourceId { get; set; }
        public Source Source { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public int AccountId { get; set; } 
        public Account? Account { get; set; } = new Account();
        public decimal? TakenFromAllowedMinus { get; set; }
    }
}
