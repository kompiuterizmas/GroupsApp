using System;
using System.Collections.Generic;

namespace GroupsApp.Api.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int PayerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public SplitType SplitType { get; set; }
        public Dictionary<int, decimal>? SplitDetails { get; set; }
    }
}
