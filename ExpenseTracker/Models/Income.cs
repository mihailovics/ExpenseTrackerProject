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
        public int SourceId { get; set; }
        public Source Source { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public int? AccountId { get; set; } 
        
        public Account? Account { get; set; } = new Account();
    }
}
