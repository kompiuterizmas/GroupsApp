using System;
using System.Collections.Generic;

namespace GroupsApp.Api.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        // Which group this transaction belongs to
        public required int GroupId { get; set; }
        public required Group Group { get; set; }

        // Who paid (must be a member of the group)
        public required int PayerUserId { get; set; }
        public required User Payer { get; set; }

        // Total amount paid
        public required decimal Amount { get; set; }

        // When the payment occurred
        public DateTime Date { get; set; }

        // Details of how the amount is split: UserId -> share (amount or percentage)
        public Dictionary<int, decimal>? SplitDetails { get; set; }
    }
}
