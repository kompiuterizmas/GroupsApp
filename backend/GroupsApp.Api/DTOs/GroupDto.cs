namespace GroupsApp.Api.DTOs
{
    public class GroupDto
    {
        // Unique identifier
        public int Id { get; set; }

        // Group title
        public string Title { get; set; } = null!;

        // Current balance (positive = you are owed, negative = you owe)
        public decimal Balance { get; set; }
    }
}
