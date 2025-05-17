using System.Collections.Generic;

namespace GroupsApp.Api.DTOs
{
    public class GroupDetailDto
    {
        // Unique group identifier
        public int Id { get; set; }

        // Group title
        public string Title { get; set; } = null!;

        // All members in this group, each with its own balance
        public List<MemberDto> Members { get; set; } = new();

        // All transactions in this group
        public List<TransactionDto> Transactions { get; set; } = new();
    }
}
