using Microsoft.AspNetCore.Server.HttpSys;

namespace ExpenseTracker.DTOs
{
    public class IncomeDTO
    {
        public decimal IncomeAmount { get; set; }

        public string? Description { get; set; }

        public string Source { get; set; } = "";

        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = ""; 

    }
}
