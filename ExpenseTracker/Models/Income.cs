using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Income
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public decimal IncomeAmount { get; set; }
        public string? Description { get; set; }
        [Required]
        public string Source { get; set; } = "";
        [Required]
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = "";
        public User? User { get; set; } = new User();
    }
}
