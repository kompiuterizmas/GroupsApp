using System.Collections.Generic;

namespace GroupsApp.Api.DTOs
{
    public class UserWithGroupsDto
    {
        // User identifier
        public int UserId { get; set; }

        // Display name
        public string Name { get; set; } = null!;

        // List of groups this user belongs to, with per-group balance
        public List<GroupBalanceDto> Groups { get; set; } = new();
    }

    public class GroupBalanceDto
    {
        // Group identifier
        public int GroupId { get; set; }

        // Group title
        public string GroupTitle { get; set; } = null!;

        // Balance for this user in that group
        public decimal Balance { get; set; }
    }
}
