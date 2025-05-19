using System.Collections.Generic;

namespace GroupsApp.Api.DTOs
{
    public class GroupDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public List<MemberDto> Members { get; set; } = new();
        public List<TransactionDto> Transactions { get; set; } = new();
    }
}
