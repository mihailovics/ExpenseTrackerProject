using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public decimal Balance { get; set; }
        [Required]
        public decimal AllowedMinus { get; set; }
        public string UserId { get; set; } = "";
        public User? User { get; set; }
        public ICollection<Income>? Incomes { get; set; }
        public ICollection<Expense>? Expenses { get; set; }
    }
}
