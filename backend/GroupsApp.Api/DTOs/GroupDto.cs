namespace GroupsApp.Api.DTOs
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal Balance { get; set; }
    }
}
