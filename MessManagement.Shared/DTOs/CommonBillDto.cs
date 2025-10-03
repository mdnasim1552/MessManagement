using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.Shared.DTOs
{
    public class CommonBillDto
    {
        public int BillId { get; set; }
        public int MessId { get; set; }
        public string BillType { get; set; }
        public decimal Amount { get; set; }
    }
}
