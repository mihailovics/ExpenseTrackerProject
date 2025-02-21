using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.DTOs
{
    public class GeneralViewModel
    {
        // Prazan konstruktor mi sluzi kad zelim sam bez u liniji da kreiram objekat klase
        public GeneralViewModel() { }
        public GeneralViewModel(decimal amount, string? description, int sourceId, List<SelectListItem>? sources, 
            DateTime createdAt, int? accountId, Account? account) { 
        
            Amount = amount;
            Description = description;
            SourceId = sourceId;
            Sources = sources;
            CreatedAt = createdAt;
            AccountId = accountId;
            Account = account;
        }
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        [Required]
        [Display(Name = "Source")]
        public int SourceId { get; set; }  
        public List<SelectListItem>? Sources { get; set; }  

        [Required]
        public DateTime CreatedAt { get; set; }

        public int? AccountId { get; set; }  
        public Account? Account { get; set; }   
    }
}
