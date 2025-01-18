using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public decimal ExpenseAmount { get; set; }
        public string? Description { get; set; }
        [Required]
        public string Source { get; set; } = "";
        [Required]
        public DateTime CreatedAt { get; set; }
        public int AccountId { get; set; } 
        public Account? Account { get; set; } = new Account();
    }
}
