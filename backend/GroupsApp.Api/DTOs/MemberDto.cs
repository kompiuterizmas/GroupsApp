namespace GroupsApp.Api.DTOs
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Balance { get; set; }
    }
}
