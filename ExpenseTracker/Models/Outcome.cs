using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Outcome
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public decimal OutcomeAmount { get; set; }
        public string? Description { get; set; }
        [Required]
        public string Source { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
