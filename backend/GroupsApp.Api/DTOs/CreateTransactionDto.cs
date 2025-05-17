namespace GroupsApp.Api.DTOs
{
    public enum SplitType
    {
        Equal,
        Percentage,
        Manual
    }

    public class CreateTransactionDto
    {
        // The member who paid
        public int PayerId { get; set; }

        // Total amount paid
        public decimal Amount { get; set; }

        // How to split: Equal, Percentage or Manual
        public SplitType SplitType { get; set; }

        // Depending on SplitType:
        // - Equal: ignore
        // - Percentage: Dictionary<MemberId, percentage>
        // - Manual: Dictionary<MemberId, amount>
        public Dictionary<int, decimal>? SplitDetails { get; set; }
    }
}
