using System.Collections.Generic;

namespace GroupsApp.Api.Models
{
    public class User
    {
        // PK – EF will generate this on add
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        // Navigation to memberships
        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        // Navigation to transactions paid by this user
        public ICollection<Transaction> PayerTransactions { get; set; } = new List<Transaction>();
    }
}
