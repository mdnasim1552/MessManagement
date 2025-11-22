namespace MessManagement.Shared.DTOs
{
    public class MessDto
    {
        public int MessId { get; set; }
        public string MessName { get; set; }
        public string? MemberNames { get; set; }
        public string? Description { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool CurrentMess { get; set; }
        public bool IsCreatedByCurrentUser { get; set; }
        public decimal TotalMarketCost { get; set; }
        public decimal TotalMeals { get; set; }
        public decimal MealRate { get; set; }
        public decimal CommonBillPerMember { get; set; }
        public List<MessMemberDto> MessMembers { get; set; } = new List<MessMemberDto>();
        public List<CommonBillDto> CommonBills { get; set; } = new List<CommonBillDto>();
    }
}
