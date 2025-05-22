using System;
using System.Collections.Generic;

namespace GroupsApp.Api.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }

        // 3. So frontend knows which group
        public int GroupId { get; set; }

        // 0. Description text
        public string Description { get; set; } = null!;

        // 1. Split type name ("Equal", "Percentage", "Manual")
        public string SplitType { get; set; } = null!;

        // 2. Amount
        public decimal Amount { get; set; }

        // 6. Date/time
        public DateTime Date { get; set; }

        // 4. Who paid (user ID)
        public int PayerId { get; set; }

        public Dictionary<int, decimal>? SplitDetails { get; set; }
    }
}
