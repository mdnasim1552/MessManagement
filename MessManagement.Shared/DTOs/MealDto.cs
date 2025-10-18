using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.Shared.DTOs
{
    public class MealDto
    {
        public int MealId { get; set; }
        public int MessId { get; set; }
        public int MessMemberId { get; set; }
        public DateOnly MealDate { get; set; }
        public decimal Breakfast { get; set; }
        public decimal Lunch { get; set; }
        public decimal Dinner { get; set; }
    }
}
