
namespace GroupsApp.Api.Models
{
    public class GroupMember
    {
        // Composite primary key: GroupId + UserId
        public int GroupId { get; set; }

        // Navigation property to Group (optional)
        public Group? Group { get; set; }

        // Foreign key to User
        public int UserId { get; set; }

        // Navigation property to User (optional)
        public User? User { get; set; }

        // You can add additional fields here, e.g. role or join date
        // public DateTime JoinedAt { get; set; }
    }
}