using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.Shared.DTOs
{
    public class MarketCostDto
    {
        public int CostId { get; set; }
        public int MessId { get; set; }
        public int MessMemberId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public int? Unit { get; set; }
        public decimal Cost { get; set; }
        public UnitDto? SelectedUnit { get; set; }

    }
}
