using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.DTOs
{
    public class ViewModel
    {
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        [Required]
        public int SourceId { get; set; }  
        public List<SelectListItem>? Sources { get; set; }  

        [Required]
        public DateTime CreatedAt { get; set; }

        public int? AccountId { get; set; }  
        public Account? Account { get; set; }   
    }
}
