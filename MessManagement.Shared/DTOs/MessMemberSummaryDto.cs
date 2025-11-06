using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.Shared.DTOs
{
    public class MessMemberSummaryDto
    {
        public int MessMemberId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TotalMeal { get; set; }
        public decimal TotalMealCost { get; set; }
        public decimal MarketCost { get; set; }
        public decimal GetOrPayFromMeal { get; set; }
        public decimal Rent { get; set; }
        public decimal TotalHaveToPay { get; set; }
        public string Email { get; set; }
        public string? Role { get; set; }
        public bool IsCreatedByCurrentUser { get; set; }
    }
}
