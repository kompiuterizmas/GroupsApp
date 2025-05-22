using System.Collections.Generic;

namespace GroupsApp.Api.DTOs
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;

        // overall balance
        public decimal Balance { get; set; }

        // so UserPage can inspect members
        public List<MemberDto> Members { get; set; } = new();
    }
}
