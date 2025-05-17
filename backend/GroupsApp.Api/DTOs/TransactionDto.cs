namespace GroupsApp.Api.DTOs
{
    public class TransactionDto
    {
        // Unique identifier
        public int Id { get; set; }

        // Who paid
        public int PayerId { get; set; }

        // Total amount
        public decimal Amount { get; set; }

        // When transaction happened
        public DateTime Date { get; set; }

        // How it was split
        public SplitType SplitType { get; set; }

        // Details per member: MemberId -> share (percentage or amount)
        public Dictionary<int, decimal>? SplitDetails { get; set; }
    }
}
