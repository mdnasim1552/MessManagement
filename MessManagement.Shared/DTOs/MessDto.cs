namespace MessManagement.Shared.DTOs
{
    public class MessDto
    {
        public int MessId { get; set; }
        public string MessName { get; set; }
        public string? Description { get; set; }
        public DateTime Month { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<MessMemberDto> MessMembers { get; set; } = new List<MessMemberDto>();
        public List<CommonBillDto> CommonBills { get; set; } = new List<CommonBillDto>();
    }
}
