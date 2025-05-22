using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupsApp.Api.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Group? Group { get; set; }

        public int PayerUserId { get; set; }
        public User? Payer { get; set; }

        // 0. Human-readable description
        public string Description { get; set; } = string.Empty;

        // 1. Which split mode was used
        public SplitType SplitType { get; set; }

        // 2. Amount paid
        public decimal Amount { get; set; }

        // 6. When it happened
        public DateTime Date { get; set; }

        // In-memory only
        [NotMapped]
        public Dictionary<int, decimal>? SplitDetails { get; set; }
    }

    public enum SplitType
    {
        Equal,
        Percentage,
        Manual
    }
}
