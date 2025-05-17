namespace GroupsApp.Api.DTOs
{
    public class MemberDto
    {
        // Unique identifier
        public int Id { get; set; }

        // Member's name
        public string Name { get; set; } = null!;

        // Member-specific balance in this group
        public decimal Balance { get; set; }
    }
}
