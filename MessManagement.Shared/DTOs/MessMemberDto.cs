namespace MessManagement.Shared.DTOs
{
    public class MessMemberDto
    {
        public int MessMemberId { get; set; }
        public int MessId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Role { get; set; }
        public DateTime? JoinedAt { get; set; }
    }
}
