using System.Collections.Generic;

namespace GroupsApp.Api.Models
{
    public class Group
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}