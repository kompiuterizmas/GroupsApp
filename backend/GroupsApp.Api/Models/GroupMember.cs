namespace GroupsApp.Api.Models
{
    public class GroupMember
    {
        // Composite PK: GroupId + UserId (configure in OnModelCreating)
        public required int GroupId { get; set; }
        public required Group Group { get; set; }

        public required int UserId { get; set; }
        public required User User { get; set; }

        // Optionally, persisted balance or role flags could go here
        // public decimal Balance { get; set; }

        // If you want quick access to splits per membership,
        // you could add navigations to split entries here.
    }
}