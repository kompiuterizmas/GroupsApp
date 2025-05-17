using System.Collections.Generic;

namespace GroupsApp.Api.Models
{
    public class User
    {
        public int Id { get; set; }

        // User's display name or username
        public required string Name { get; set; }

        // All group memberships for this user
        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

        // All transactions where this user was the payer
        public ICollection<Transaction> PayerTransactions { get; set; } = new List<Transaction>();
    }
}
